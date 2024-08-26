
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Inoa
{
      
class Program
{
      static async Task Main(){
            List<string> fileEmail = ReadFile();
            var user =  getUserInformation();
            // Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();
            var jobDataMap = new JobDataMap();
            jobDataMap["active"] = user.active;
            jobDataMap["buyValue"] = user.buyValue;
            jobDataMap["sellValue"] = user.sellValue;

            IJobDetail job = JobBuilder.Create<ActiveJob>()
                .WithIdentity("job1", "group1")
                .SetJobData(jobDataMap)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            await Task.Delay(TimeSpan.FromSeconds(60));

            await scheduler.Shutdown();

            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
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

            static readonly HttpClient client = new HttpClient();
            static async Task<decimal> getActivePrices (string active){
                  decimal price = 0;
                  try
                  {        
                        Console.WriteLine(active);
                        HttpResponseMessage response = await client.GetAsync($"https://brapi.dev/api/quote/{active}?modules=summaryProfile&token=rQAmrzQdzbNxLddDqxrn4A");
                        response.EnsureSuccessStatusCode();
                        string responseData = await response.Content.ReadAsStringAsync();
                        JsonNode data = JsonNode.Parse(responseData);
                        JsonNode root = data.Root;
                        JsonArray result = root["results"]!.AsArray();
                        JsonValue prices = result[0]?["regularMarketPrice"].AsValue();
                        price = (decimal)prices;
                        return price;
                  }
                  catch (HttpRequestException e)
                  {
                    Console.WriteLine($"Erro na requisição: {e.Message}");
                  }
                  return price;
            }

           static string emailValidation (decimal price, int buyValue, int sellValue){
            Console.WriteLine(price);
            if(price <= buyValue){
                  Console.WriteLine("Comprar");
                  return "Comprar";
            }
            else if (price >= sellValue ){
                  Console.WriteLine("Vender");
                  return "Vender";
            
            }
            
            return "";
           }

           static List<string> ReadFile () {
            List <string> list =[""];
            try
            {
                  string data = File.ReadAllText("text.txt");
                  list = data.Split("\r\n").ToList();
                  Console.WriteLine(list[1]);
            }
            catch (System.Exception e)
            {
                  Console.WriteLine($"Ocorreu um erro: {e.Message}");
            }
            return list;
           }
           public class ActiveJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            string active = context.JobDetail.JobDataMap.GetString("active");
            int buyValue = context.JobDetail.JobDataMap.GetInt("buyValue");
            int sellValue  = context.JobDetail.JobDataMap.GetInt("sellValue");
            var price = await getActivePrices(active);
            string comparation = emailValidation(price, buyValue, sellValue);
            Console.WriteLine($"Parametros {active}, {buyValue}, {sellValue}, {comparation}");
        }
    }

}
 

 }

// 
