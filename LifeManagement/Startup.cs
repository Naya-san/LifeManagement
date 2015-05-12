using LifeManagement.Logic;
using LifeManagement.ViewModels;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(LifeManagement.Startup))]

namespace LifeManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
            TodoListTimeTicker ticker = new TodoListTimeTicker();
         //   ForTestTicker testTicker = new ForTestTicker();
            // Дополнительные сведения о настройке приложения см. по адресу: http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
