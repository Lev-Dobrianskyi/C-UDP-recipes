using System.Net;
using System.Net.Sockets;
using System.Text;

class Food {
    private string name;
    private string[] ingredients;
    public Food(string name, string[] ingredients)
    {
        this.name = name;
        this.ingredients = ingredients;
    }
    public
    string[] getIngredients() {
        return ingredients;
    }
    public
    string getName()
    {
        return name;
    }
};

class Server
{
    static Food applePie = new Food("Apple pie", ["Apples", "Dough"]);
    static Food roastedChickenLiver = new Food("Roasted chicken liver", ["Chicken liver", "Sour cream", "Salt", "Onion"]);
    static Food pizza = new Food("Cheese pizza", ["Dough", "Cheese"]);
    static Food salad = new Food("Salad", ["Lettuce", "Tomato", "Cucumber", "Onion"]);
    static Food goodMashedPotatoes = new Food("GoodMashedPotatoes", ["Potato", "Onion"]);
    static Food[] recipes = {applePie, roastedChickenLiver, pizza, salad, goodMashedPotatoes};
    static void Main(string[] args)
    {
        bool foundRecipe;

        Socket reciever = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 6000);
        reciever.Bind(endPoint);

        while (true)
        {
            List<string> chosenIngredients = new List<string> { };
            foundRecipe = false;
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 6000);

            byte[] buffer = new byte[4096];
            while (true)
            {
                int size = reciever.ReceiveFrom(buffer, ref remoteEndPoint);
                string text = Encoding.UTF8.GetString(buffer, 0, size);
                Console.WriteLine("From client: " + text);
                if (!(text == "End of ingredients list"))
                {
                    chosenIngredients.Add(text); // recieve every ingredient, ingredient by ingredient
                    continue;
                }
                break;
            }
            for (int i = 0; i < recipes.Length; i++)
            {
                //for (int j = 0; j < chosenIngredients.Count; j++)
                //{
                    //if (recipes[i].getIngredients().Contains(chosenIngredients[j]))
                    if (chosenIngredients.All(ingredient => recipes[i].getIngredients().Contains(ingredient))) // if every single element in chosenIngredients exists in recipes[i] (a single element is the "ingredient" variable) - return true, else - return false
                    {
                        foundRecipe = true;
                        //byte[] data = Encoding.UTF8.GetBytes(recipes[i].getName()); you could just send ONLY the name, if you want to
                        foreach (string ingredient in recipes[i].getIngredients())
                        {
                            byte[] data = Encoding.UTF8.GetBytes(ingredient); // send the whole matching recipe ingredient by ingredient
                            reciever.SendTo(
                                data,
                                remoteEndPoint
                                );
                        }
                        byte[] data2 = Encoding.UTF8.GetBytes("---------------------------------------");
                        reciever.SendTo(
                            data2,
                            remoteEndPoint
                            );
                        //break;
                    }
                //}
            }
            if (foundRecipe == false)
            {
                byte[] data = Encoding.UTF8.GetBytes("No recipes");
                reciever.SendTo(
                    data,
                    remoteEndPoint
                    );
            }
            else
            {
                byte[] data = Encoding.UTF8.GetBytes("Sent all recipes");
                reciever.SendTo(
                    data,
                    remoteEndPoint
                    );
            }
        }
    }
}
