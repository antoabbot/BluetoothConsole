using Nito.AsyncEx;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

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
            var baseGuid = new Guid("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
            var txGuid =   new Guid("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
            var rxGuid =   new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e");



            var selector = GattDeviceService.GetDeviceSelectorFromUuid(baseGuid);
            var services = await DeviceInformation.FindAllAsync(selector);
            var id = services[0].Id;
            var name = services[0].Name;
            Console.WriteLine("Using Service: {0}", name);

            var service = await GattDeviceService.FromIdAsync(id);

            var gattTx = service.GetCharacteristics(txGuid)[0];
            var txProps = gattTx.CharacteristicProperties;
            var isWWR = txProps.HasFlag(GattCharacteristicProperties.WriteWithoutResponse);
            var isWR = txProps.HasFlag(GattCharacteristicProperties.Write);
            var isRR = txProps.HasFlag(GattCharacteristicProperties.ReliableWrites);
            var isSR = txProps.HasFlag(GattCharacteristicProperties.AuthenticatedSignedWrites);
            Console.WriteLine("Tx :" + gattTx.CharacteristicProperties);
            var gattRx = service.GetCharacteristics(rxGuid)[0];
            var rxProps = gattRx.CharacteristicProperties;
            var isIR = rxProps.HasFlag(GattCharacteristicProperties.Read);
            var isRN = rxProps.HasFlag(GattCharacteristicProperties.Notify);
            var isRI = rxProps.HasFlag(GattCharacteristicProperties.Indicate);
            var isRB = rxProps.HasFlag(GattCharacteristicProperties.Broadcast);
            Console.WriteLine("Rx :" + gattRx.CharacteristicProperties);

            gattRx.ValueChanged += GattRx_ValueChanged;
            //            var val = await gattRx.ReadValueAsync(Windows.Devices.Bluetooth.BluetoothCacheMode.Cached);

            Console.WriteLine("Callback enabled. Type stuff to send");

            string data = string.Empty;
            while (data.ToUpper() != "Q")
            {
                data = Console.ReadLine();

                var writer = new DataWriter();
                writer.WriteString(data);
                var res = await gattTx.WriteValueAsync(writer.DetachBuffer(), GattWriteOption.WriteWithResponse);

                Console.Write(data);
                Console.WriteLine(res);
            }
        }

        private static void GattRx_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            Console.WriteLine(args.CharacteristicValue.ToString());
        }
    }
}
