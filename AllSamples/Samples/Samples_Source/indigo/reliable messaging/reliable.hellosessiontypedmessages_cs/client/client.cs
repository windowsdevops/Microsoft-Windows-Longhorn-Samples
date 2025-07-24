using System;
using System.MessageBus;
using System.MessageBus.Services;
// the namespace of the imported service interface
using www_tempuri_org.quickstarts;

namespace Microsoft.Samples.MessageBus.Quickstarts
{
    public class Client 
    {
        public static void Main(string[] args) 
        {
            string name = "Client";
            if (args.Length > 0)
                name = args[0];

            // Load the default service environment, called "main".
            ServiceEnvironment se = ServiceEnvironment.Load();

            // Retrieve the ServiceManager from the default environment
            ServiceManager manager = se[typeof(ServiceManager)] as ServiceManager;
            if (manager == null)
            {
                throw new ApplicationException(
                    "The ServiceManager is not available in the service se for some reason."
                    );
            }
            // Start the service environment.
            se.Open();

            // Create a proxy channel that points to the service to call.
            Uri uri = new Uri("soap.tcp://localhost:46000/HelloService/");
            IHelloChannel channel = (IHelloChannel)manager.CreateChannel(typeof(IHelloChannel), uri);

            try 
            {
                HelloResponse response = channel.Greeting(new HelloSalutation(name));
                Console.WriteLine(response.Response);
                response = channel.Greeting(new HelloSalutation(name + ": Second time around"));
                Console.WriteLine(response.Response);
                response = channel.Greeting(new HelloSalutation(name + ": Third time around"));
                Console.WriteLine(response.Response);
                response = channel.Greeting(new HelloSalutation(name + ": Fourth time around"));
                Console.WriteLine(response.Response);
                channel.DoneSending();
            }
            catch(Exception Ex)
            {
                Console.WriteLine(Ex);
            }
            finally
            {
                se.Close();
            }
        }

    }
}