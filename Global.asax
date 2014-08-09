<%@ Application Language="C#" %>
<%@ Import Namespace="code" %>
<%@ Import Namespace="System.Web.Routing" %>

<script RunAt="server">

    protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    void Application_Start(object sender, EventArgs e)
    {
        // 在应用程序启动时运行的代码
        AuthConfig.RegisterOpenAuth();
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        //  在应用程序关闭时运行的代码
        //[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
        log4net.Config.XmlConfigurator.Configure();
    }

    void Application_End(object sender, EventArgs e)
    {
    }

    void Application_Error(object sender, EventArgs e)
    {
        // 在出现未处理的错误时运行的代码
        _log.Error(sender == null ? string.Empty : sender.ToString() + "\r\n\t" + e);
    }

</script>
