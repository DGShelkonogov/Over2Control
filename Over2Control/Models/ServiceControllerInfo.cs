using System;
using System.ServiceProcess;

namespace Over2Control.Models
{
    public class ServiceControllerInfo
    {
        private readonly ServiceController _controller;

        public ServiceControllerInfo(ServiceController controller)
        {
            _controller = controller;
        }

        public ServiceController Controller
        {
            get { return _controller; }
        }

        public string ServiceTypeName
        {
            get
            {
                var type = _controller.ServiceType;
                string serviceTypeName = String.Empty;
                if ((type & ServiceType.InteractiveProcess) != 0)
                {
                    serviceTypeName = "Interactive ";
                    type -= ServiceType.InteractiveProcess;
                }
                switch (type)
                {
                    case ServiceType.Adapter:
                        serviceTypeName += "Adapter";
                        break;

                    case ServiceType.FileSystemDriver:
                    case ServiceType.KernelDriver:
                    case ServiceType.RecognizerDriver:
                        serviceTypeName += "Driver";
                        break;

                    case ServiceType.Win32OwnProcess:
                        serviceTypeName += "Win32 Service Process";
                        break;

                    case ServiceType.Win32ShareProcess:
                        serviceTypeName += "Win32 Shared Process";
                        break;

                    default:
                        serviceTypeName += "unknown type " + type.ToString();
                        break;
                }
                return serviceTypeName;
            }
        }

        public string ServiceStatusName
        {
            get
            {
                switch (_controller.Status)
                {
                    case ServiceControllerStatus.ContinuePending:
                        return "Continue Pending";
                    case ServiceControllerStatus.Paused:
                        return "Paused";
                    case ServiceControllerStatus.PausePending:
                        return "Pause Pending";
                    case ServiceControllerStatus.StartPending:
                        return "Start Pending";
                    case ServiceControllerStatus.Running:
                        return "Running";
                    case ServiceControllerStatus.Stopped:
                        return "Stopped";
                    case ServiceControllerStatus.StopPending:
                        return "Stop Pending";
                    default:
                        return "Unknown status";
                }
            }
        }

        public string DisplayName
        {
            get { return _controller.DisplayName; }
        }

        public string ServiceName
        {
            get { return _controller.ServiceName; }
        }

        public bool EnableStart
        {
            get
            {
                return _controller.Status == ServiceControllerStatus.Stopped;
            }
        }

        public bool EnableStop
        {
            get
            {
                return _controller.Status == ServiceControllerStatus.Running;
            }
        }

        public bool EnablePause
        {
            get
            {
                return _controller.Status == ServiceControllerStatus.Running &&
                      _controller.CanPauseAndContinue;
            }
        }

        public bool EnableContinue
        {
            get
            {
                return _controller.Status == ServiceControllerStatus.Paused;
            }
        }

    }
}
