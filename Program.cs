

class Program
{
      static void Main(){
          var user =  getUserInformation();
          Console.WriteLine(user.buyValue);
          
      }

      static (string active, int buyValue, int sellValue ) getUserInformation() {
            string active =""; 
            int buyValue = 0;
            int sellValue = 0;
            try
            {
                  Console.Write("Digite o nome da ação: ");
                  active = Console.ReadLine();

                  Console.Write("Digite o valor de compra da ação: ");
                  buyValue = int.Parse(Console.ReadLine());

                  Console.Write("Digite o valor de venda da ação: ");
                  sellValue = int.Parse(Console.ReadLine());

                  return (active, buyValue, sellValue);
            }
            catch (System.Exception exception)
            {
                  Console.WriteLine(" ");
                  Console.WriteLine($"Ocorreu um erro no programa {exception.Message} Por favor verifique se você digitou os parametros corretos!");
            }
            
            return (active, buyValue, sellValue);
            
            }
}



