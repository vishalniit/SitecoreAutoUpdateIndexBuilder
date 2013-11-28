using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace AutoUpdateIndexer
{
    public static class UtilitySettings
    {
        /// <summary>
        /// Gets the application SMTP Server.
        /// </summary>
        /// <value>The application SMTP Value.</value>
        public static string SMTP
        {
            get
            {
                return ValidationHelper.ValidateToString(ConfigurationManager.AppSettings["Host"], "");
            }
        }

        /// <summary>
        /// Gets the application SMTP Server Outgoing Port.
        /// </summary>
        /// <value>The Outgoing Port.</value>
        public static int OutgoingPort
        {
            get
            {
                return ValidationHelper.ValidateToInt(ConfigurationManager.AppSettings["Port"], 0);
            }
        }

        /// <summary>
        /// Gets the application SMTP Server Username.
        /// </summary>
        /// <value>The Username.</value>
        public static string SMTPUserName
        {
            get
            {
                return ValidationHelper.ValidateToString(ConfigurationManager.AppSettings["SMTPUserName"], "");
            }
        }

        /// <summary>
        /// Gets the application SMTP Server Password.
        /// </summary>
        /// <value>The Password.</value>
        public static string SMTPPassword
        {
            get
            {
                return ValidationHelper.ValidateToString(ConfigurationManager.AppSettings["SMTPPassword"], "");
            }
        }

        /// <summary>
        /// Gets the application SMTP Server SSLEnable.
        /// </summary>
        /// <value>The SSLEnable.</value>
        public static bool SSLEnable
        {
            get
            {
                return ValidationHelper.ValidateToBoolean(ConfigurationManager.AppSettings["SSLEnable"], false);
            }
        }

        /// <summary>
        /// Gets the application SMTP Server IsAuthentication.
        /// </summary>
        /// <value>The SSLEnable.</value>
        public static bool IsAuthentication
        {
            get
            {
                return ValidationHelper.ValidateToBoolean(ConfigurationManager.AppSettings["IsAuthentication"], false);
            }
        }

        /// <summary>
        /// How much time to wait for SMTP to reply.
        /// </summary>
        /// <value>The SMTPAvailabilityWaitTime.</value>
        public static int SMTPAvailabilityWaitTime
        {
            get
            {
                return ValidationHelper.ValidateToInt(ConfigurationManager.AppSettings["SMTPAvailabilityWaitTime"], 500);
            }
        }

        /// <summary>
        /// Get CSO email(s) from configuration
        /// </summary>
        public static string CSOEmail
        {
            get
            {
                return ValidationHelper.ValidateToString(ConfigurationManager.AppSettings["CSOEmail.ToAddress"], "");
            }
        }

        /// <summary>
        /// Get Content Index Name from configuration
        /// </summary>
        public static string ContentIndexName
        {
            get
            {
                return ValidationHelper.ValidateToString(ConfigurationManager.AppSettings["ContentIndexName"], "");
            }
        }

        /// <summary>
        /// Get Global Referfence Index Name from configuration
        /// </summary>
        public static string GlobalReferenceIndexName
        {
            get
            {
                return ValidationHelper.ValidateToString(ConfigurationManager.AppSettings["GlobalReferenceIndexName"], "");
            }
        }

        /// <summary>
        /// Get Product Index Name from configuration
        /// </summary>
        public static string ProductIndexName
        {
            get
            {
                return ValidationHelper.ValidateToString(ConfigurationManager.AppSettings["ProductIndexName"], "");
            }
        }

        /// <summary>
        /// Get File Index Name from configuration
        /// </summary>
        public static string FileIndexName
        {
            get
            {
                return ValidationHelper.ValidateToString(ConfigurationManager.AppSettings["FileIndexName"], "");
            }
        }

        /// <summary>
        /// Get OuterIndexFolder Name from configuration
        /// </summary>
        public static string OuterIndexFolder
        {
            get
            {
                return ValidationHelper.ValidateToString(ConfigurationManager.AppSettings["OuterIndexFolder"], "");
            }
        }

        /// <summary>
        /// Get AutoUpdateIndexName Name from configuration
        /// </summary>
        public static string AutoUpdateIndexName
        {
            get
            {
                return ValidationHelper.ValidateToString(ConfigurationManager.AppSettings["AutoUpdateIndexName"], "");
            }
        }

        /// <summary>
        /// Get MobileBillPlan Path from configuration
        /// </summary>
        public static string MobileBillPlanPath
        {
            get
            {
                return ValidationHelper.ValidateToString(ConfigurationManager.AppSettings["MobileBillPlanPath"], "");
            }
        }


        /// <summary>
        /// How many result search should return.
        /// </summary>
        /// <value>The result count in search.</value>
        public static int MaximumResultCountAllowed
        {
            get
            {
                return ValidationHelper.ValidateToInt(ConfigurationManager.AppSettings["MaximumResultCountAllowed"], 200);
            }
        }

        
        /// <summary>
        /// What length of minimum word will be indexed.
        /// </summary>
        /// <value>The result count in search.</value>
        public static int AllowedMinimumWordLengthToBeIndexed
        {
            get
            {
                return ValidationHelper.ValidateToInt(ConfigurationManager.AppSettings["AllowedMinimumWordLengthToBeIndexed"], 3);
            }
        }

        /// <summary>
        /// What length of maximum word will be indexed.
        /// </summary>
        /// <value>The result count in search.</value>
        public static int AllowedMaxWordLengthToBeIndexed
        {
            get
            {
                return ValidationHelper.ValidateToInt(ConfigurationManager.AppSettings["AllowedMaxWordLengthToBeIndexed"], 45);
            }
        }
    }
}
