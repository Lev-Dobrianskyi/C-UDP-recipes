using System.Net;
using System.Net.Sockets;
using System.Text;
class Client
{
    static void Main(string[] args)
    {
        List<string> recipes = new List<string> { };

        Socket reciever = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp
            );

        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);

        while (true)
        {
            Console.WriteLine("How many different ingredients do you have?");
            string amount = Console.ReadLine();
            List<string> ingredients = new List<string> { };
            for (int i = 0; i < int.Parse(amount); i++)
            {
                Console.WriteLine("Your " + (i + 1) + "th ingredient");
                ingredients.Add(Console.ReadLine());
            }
            foreach (string ingredient in ingredients)
            {
                Console.WriteLine("Sent to server: " + ingredient);
                byte[] data = Encoding.UTF8.GetBytes(ingredient);
                reciever.SendTo(data, remoteEndPoint);
            }
            byte[] dataEnd = Encoding.UTF8.GetBytes("End of ingredients list");
            reciever.SendTo(dataEnd, remoteEndPoint);

            byte[] buffer = new byte[4096];
            while (true)
            {
                int size = reciever.ReceiveFrom(buffer, ref remoteEndPoint);
                string text = Encoding.UTF8.GetString(buffer, 0, size);
                Console.WriteLine("From server: " + text);
                if (text == "No recipes" || text == "Sent all recipes")
                {
                    break;
                }
            }
        }
    }
}
