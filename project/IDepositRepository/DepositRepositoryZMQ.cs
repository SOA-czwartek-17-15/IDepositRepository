using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contracts;
using System.Threading;
using ZMQ;

using Newtonsoft.Json;

namespace DepositService
{
    internal class DepositRepositoryZMQ
    {

        // since we create a wrapper for DepositRepository, we need a reference to it as a field

        private readonly DepositRepository _depositRepository;
        private readonly string _address;
        private Thread _workerThread;
        private bool _stop = false;
        private object _locker = new object();
        
        // the same IP and port as for web service?

        public DepositRepositoryZMQ(DepositRepository depositRepository, String address)
        {
            _depositRepository = depositRepository;
            _address = address;
        }

        // a main message queue server method, ran in a separate thread

        private void RunZeroMqServer()
        {
            using (var context = new Context())
            using (var server = context.Socket(SocketType.REP))
            {

                server.Bind(_address.ToString());

                // until server has been stopped, read messages each 100 ms and process them

                while (!_stop)
                {
                    string message = server.Recv(Encoding.Unicode, 100);
                   
                    // if nothing read, continue

                    if (message == null)
                    {
                        continue;
                    }

                    var response = ProcessMessage(message);

                    // send response (serialized internally in ProcessMessage)

                    server.Send(response, Encoding.Unicode);

                    Thread.Sleep(100);
                }
            }
        }

        private string ProcessMessage(string message)
        {
            var accountRepositoryMessage = JsonConvert.DeserializeObject<DepositRepositoryMessage>(message);

            String response = null;

            if (accountRepositoryMessage.Action == "CreateDepositForClient")
            {
                // deserialize arguments

                var createDepositForClientMessage =
                JsonConvert.DeserializeObject<CreateDepositForClientMessage>(accountRepositoryMessage.Arguments);

                response =
                JsonConvert.SerializeObject(
                    _depositRepository.CreateDepositForClient(
                        createDepositForClientMessage.ClientId,
                        createDepositForClientMessage.Deposit
                    )                
                );
            }
            else if (accountRepositoryMessage.Action == "CreateDepositForAccount")
            {
                // deserialize arguments

                var createDepositForAccountMessage =
                JsonConvert.DeserializeObject<CreateDepositForAccountMessage>(accountRepositoryMessage.Arguments);

                response = 
                JsonConvert.SerializeObject(
                    _depositRepository.CreateDepositForAccount(
                        createDepositForAccountMessage.AccountNumber,
                        createDepositForAccountMessage.Deposit
                    )
                );
            }
            else if (accountRepositoryMessage.Action == "GetDepositInformation")
            {
                // deserialize arguments

                var getDepositInformationMessage =
                JsonConvert.DeserializeObject<GetDepositInformationMessage>(accountRepositoryMessage.Arguments);

                response =
                JsonConvert.SerializeObject(
                    _depositRepository.GetDepositInformation(
                        getDepositInformationMessage.DepositId
                    )
                );
            }
            else if (accountRepositoryMessage.Action == "RemoveDeposit")
            {
                // deserialize arguments

                var removeDepositMessage =
                JsonConvert.DeserializeObject<RemoveDepositMessage>(accountRepositoryMessage.Arguments);

                _depositRepository.RemoveDeposit(removeDepositMessage.DepositId);

                response = "";
            }
            else
            {
                throw new InvalidOperationException(string.Format("Cannot process unknown action {0}", accountRepositoryMessage.Action));
            }
            
            return response;
        }
       
        // Start() creates and runs MQ in new thread

        public void Start()
        {
            _workerThread = new Thread(RunZeroMqServer);
            _workerThread.Start();
        }

        // stops entire ZMQ server

        public void Stop()
        {
            lock (_locker)
            {
                _stop = true;
            }

            _workerThread.Join();
        }
    }
}
