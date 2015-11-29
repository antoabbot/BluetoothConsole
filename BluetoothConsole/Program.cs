using Nito.AsyncEx;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;

namespace BluetoothConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync(args));

        }

        static async void MainAsync(string[] args)
        {
            var guid = new Guid("6E400001-B5A3-F393-E0A9-E50E24DCCA9E");
            var selector = GattDeviceService.GetDeviceSelectorFromUuid(guid);

            var services = await DeviceInformation.FindAllAsync(selector);
            var id = services[0].Id;
            var service = await GattDeviceService.FromIdAsync(id);
            var gattCharacteristic = service.GetCharacteristics(guid)[0];

//            var writer = new DataWriter();
//            writer.WriteString("#FF00FF");
//            var res = await gattCharacteristic.WriteValueAsync(writer.DetachBuffer(), GattWriteOption.WriteWithoutResponse);

        }
    }
}
