using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using System;
using System.IO;
using System.Reflection;

namespace Itac4Lear
{
    internal static class Logger
    {
        public static ILog Log;

        public static bool Init()
        {
            bool flag;
            try
            {
                if (Logger.Log == null)
                {
                    Logger.Setup();
                    Logger.Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                }
                Logger.Log.Info((object)" ---------- SeicaItac4Lear Init ---------- ");
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
            }
            return flag;
        }

        public static bool Term()
        {
            bool flag;
            try
            {
                Logger.Log.Info((object)" ---------- SeicaItac4Lear Term ---------- ");
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            return flag;
        }

        private static void Setup()
        {
            log4net.Repository.Hierarchy.Hierarchy repository = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%newline%date T:[%thread] L:[%-5level] => %message %newline";
            ((LayoutSkeleton)patternLayout).ActivateOptions();
            RollingFileAppender rollingFileAppender = new RollingFileAppender();
            ((FileAppender)rollingFileAppender).AppendToFile = true;
            ((FileAppender)rollingFileAppender).File = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SeicaItac4Lear.Log");
            ((AppenderSkeleton)rollingFileAppender).Layout = patternLayout;
            rollingFileAppender.MaxSizeRollBackups = 5;
            rollingFileAppender.MaximumFileSize = "10MB";
            rollingFileAppender.RollingStyle = RollingFileAppender.RollingMode.Size;
            rollingFileAppender.StaticLogFileName = true;
            ((TextWriterAppender)rollingFileAppender).ImmediateFlush = true;
            ((AppenderSkeleton)rollingFileAppender).ActivateOptions();
            repository.Root.AddAppender((IAppender)rollingFileAppender);
            repository.Root.Level = Level.Info;
            ((LoggerRepositorySkeleton)repository).Configured = true;
        }
    }
}
