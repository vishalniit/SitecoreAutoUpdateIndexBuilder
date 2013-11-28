using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Search;
using SpellChecker.Net.Search.Spell;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.NGram;
using Lucene.Net.Documents;
using log4net;

namespace AutoUpdateIndexer
{
    /// <summary>
    /// Search term auto-completer, works for single terms (so use on the last term of the query).
    /// Returns more popular terms first.
    /// <br/>
    /// Author: Mat Mannion, M.Mannion@warwick.ac.uk
    /// <seealso cref="http://stackoverflow.com/questions/120180/how-to-do-query-auto-completion-suggestions-in-lucene"/>
    /// </summary>
    /// 
    public class SearchAutoComplete
    {
        public static ILog log = log4net.LogManager.GetLogger(typeof(SearchAutoComplete));
        public int MaxResults { get; set; }

        private static readonly Lucene.Net.Util.Version kLuceneVersion = Lucene.Net.Util.Version.LUCENE_CURRENT;

        private static readonly String kGrammedWordsField = "words";

        private static readonly String kSourceWordField = "sourceWord";

        private static readonly String kCountField = "count";

        private static readonly String[] kEnglishStopWords = {
            "a", "an", "and", "are", "as", "at", "be", "but", "by",
            "for", "i", "if", "in", "into", "is",
            "no", "not", "of", "on", "or", "s", "such",
            "t", "that", "the", "their", "then", "there", "these",
            "they", "this", "to", "was", "will", "with"
        };

        public bool IsFirstTime { get; set; }

        private readonly Directory m_directory;

        private IndexReader m_reader;

        private IndexSearcher m_searcher;

        public SearchAutoComplete(string autoCompleteDir) :
            this(FSDirectory.Open(new System.IO.DirectoryInfo(autoCompleteDir)))
        {
        }

        public SearchAutoComplete(Directory autoCompleteDir, int maxResults = 8)
        {
            this.m_directory = autoCompleteDir;
            MaxResults = maxResults;

            ReplaceSearcher();
        }

        /// <summary>
        /// Find terms matching the given partial word that appear in the highest number of documents.</summary>
        /// <param name="term">A word or part of a word</param>
        /// <returns>A list of suggested completions</returns>
        public string[] SuggestTermsFor(string term)
        {
            if (m_searcher == null)
                return new string[] { };

            // get the top terms for query
            Query query = new TermQuery(new Term(kGrammedWordsField, term.ToLower()));

            Sort sort = new Sort(new SortField(kCountField, SortField.INT));

            TopDocs docs = m_searcher.Search(query, null, MaxResults, sort);
            string[] suggestions = docs.ScoreDocs.Select(doc =>
                m_reader.Document(doc.doc).Get(kSourceWordField)).ToArray();

            return suggestions;
        }

        /// <summary>
        /// Open the index in the given directory and create a new index of word frequency for the 
        /// given index.</summary>
        /// <param name="sourceDirectory">Directory containing the index to count words in.</param>
        /// <param name="fieldToAutocomplete">The field in the index that should be analyzed.</param>
        public void BuildAutoCompleteIndex(Directory sourceDirectory, Directory TargetDirectory, bool verbose)
        {
            // build a dictionary (from the spell package)
            using (IndexReader sourceReader = IndexReader.Open(sourceDirectory, true))
            {

                string[] fieldNames = sourceReader.GetFieldNames(IndexReader.FieldOption.ALL).ToArray();
                foreach (string fieldToAutocomplete in fieldNames)
                {
                    if (fieldToAutocomplete.Contains("__display name") || fieldToAutocomplete.Contains("_name") || !fieldToAutocomplete.Contains('_') || !fieldToAutocomplete.Contains("date") || !fieldToAutocomplete.Contains("threshold"))
                    {
                        LuceneDictionary dict = new LuceneDictionary(sourceReader, fieldToAutocomplete);

                        // code from
                        // org.apache.lucene.search.spell.SpellChecker.indexDictionary(
                        // Dictionary)
                        //IndexWriter.Unlock(m_directory);

                        // use a custom analyzer so we can do EdgeNGramFiltering
                        AutoCompleteAnalyzer analyzer = new AutoCompleteAnalyzer();
                        using (var writer = new IndexWriter(TargetDirectory, analyzer, IsFirstTime, IndexWriter.MaxFieldLength.UNLIMITED))
                        {
                            writer.SetMergeFactor(300);
                            writer.SetMaxBufferedDocs(150);

                            // go through every word, storing the original word (incl. n-grams) 
                            // and the number of times it occurs
                            System.Collections.IEnumerator ie = dict.GetWordsIterator();
                            double num;
                            Guid guid;
                            foreach (string word in dict)
                            {
                                if (word.Length < UtilitySettings.AllowedMinimumWordLengthToBeIndexed)
                                    continue; // too short we bail but 
                                if (word.Length > UtilitySettings.AllowedMaxWordLengthToBeIndexed)
                                    continue; //too long also we bail out


                                if (!word.Contains('<') && !word.Contains('>') && !word.Contains('/') && !word.Contains('\\') && !isNotFile(word) && !word.Contains('@') && !word.Contains('&') && !double.TryParse(word, out num) && !Guid.TryParse(word, out guid))
                                {
                                    // ok index the word
                                    // use the number of documents this word appears in
                                    int freq = sourceReader.DocFreq(new Term(fieldToAutocomplete, word));
                                    if (verbose)
                                    {                                        
                                        log.Info(string.Format("Frequency {0} of this word {1}", freq, word));
                                    }
                                    var doc = MakeDocument(fieldToAutocomplete, word, freq);
                                    writer.AddDocument(doc);
                                }
                            }
                            writer.Optimize();
                        }
                    }
                }
            }

            // re-open our reader
            //ReplaceSearcher();
        }

        private static Document MakeDocument(String fieldToAutocomplete, string word, int frequency)
        {
            var doc = new Document();
            doc.Add(new Field(kSourceWordField, word, Field.Store.YES, Field.Index.NOT_ANALYZED)); // orig term
            doc.Add(new Field(kGrammedWordsField, word, Field.Store.YES, Field.Index.ANALYZED)); // grammed
            doc.Add(new Field(kCountField, frequency.ToString(), Field.Store.NO, Field.Index.NOT_ANALYZED)); // count
            return doc;
        }

        private void ReplaceSearcher()
        {
            if (IndexReader.IndexExists(m_directory))
            {
                if (m_reader == null)
                    m_reader = IndexReader.Open(m_directory, true);
                else
                    m_reader.Reopen();

                m_searcher = new IndexSearcher(m_reader);
            }
            else
            {
                m_searcher = null;
            }
        }

        private bool isNotFile(string word)
        {
            bool result = false;
            if (word == null || word.Length < 1)
                return result;

            if (word.Contains(".png") || word.Contains(".jpeg") || word.Contains(".jpg") || word.Contains(".gif") || word.Contains(".tif") || word.Contains(".ico") || word.Contains(".bmp") || word.Contains(".aspx") || word.Contains("&amp"))
            {
                result = true;
            }
            return result;
        }
    }

    public class AutoCompleteAnalyzer : Analyzer
    {
        private static readonly Lucene.Net.Util.Version kLuceneVersion = Lucene.Net.Util.Version.LUCENE_24;

        private static readonly String[] kEnglishStopWords = {
            "a", "an", "and", "are", "as", "at", "be", "but", "by",
            "for", "i", "if", "in", "into", "is",
            "no", "not", "of", "on", "or", "s", "such",
            "t", "that", "the", "their", "then", "there", "these",
            "they", "this", "to", "was", "will", "with"
        };

        public override TokenStream TokenStream(string fieldName, System.IO.TextReader reader)
        {
            TokenStream result = new StandardTokenizer(kLuceneVersion, reader);

            result = new StandardFilter(result);
            result = new LowerCaseFilter(result);
            result = new ASCIIFoldingFilter(result);
            result = new StopFilter(false, result, StopFilter.MakeStopSet(kEnglishStopWords));
            result = new EdgeNGramTokenFilter(
                result, Lucene.Net.Analysis.NGram.EdgeNGramTokenFilter.Side.FRONT, 1, 20);

            return result;
        }
    }
}
