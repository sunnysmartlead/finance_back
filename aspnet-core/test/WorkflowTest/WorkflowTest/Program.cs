namespace WorkflowTest
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();


#if DEBUG
            UserInfo.Url = "https://localhost:44311";
#else
            UserInfo.Url = "http://139.196.216.165:44333";

#endif


            Application.Run(new Form1());
        }
    }
}