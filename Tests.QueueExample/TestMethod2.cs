using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.QueueExample
{
    public class TestMethod2
    {
        public static ConcurrentQueue<Person> ConcurrenPersons = new ConcurrentQueue<Person>();

        public void MockTest()
        {
            // 注册Timer 在web项目中可以在 ApplicationStart 或者 静态构造函数中注册
            TimeTask.Instance().ExecuteTask += new System.Timers.ElapsedEventHandler(ExecuteTask);
            // 修改成每隔一秒执行一次
            TimeTask.Instance().Interval = 1000 * 1;
            TimeTask.Instance().Start();

            // 模拟入队
            var model1 = new Person("宋江", "男", 66);
            var model2 = new Person("李逵", "男", 53);
            var model3 = new Person("顾大嫂", "女", 46);
            var model4 = new Person("扈三娘", "女", 46);
            var model5 = new Person("一丈青", "女", 36);
            var model6 = new Person("林冲", "男", 45);
            var model7 = new Person("武松", "男", 42);
            var model8 = new Person("花和尚", "男", 39);
            var listPerson = new List<Person> { model1, model2, model3, model4, model5, model6, model7, model8 };

            // 开始排队
            foreach (var item in listPerson)
            {
                PersonEnqueue(item);
            }
            // 排队完成

           

            Console.ReadKey();
        }


        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="model"></param>
        public static void PersonEnqueue(Person model)
        {
            ConcurrenPersons.Enqueue(model);
        }

        /// <summary>
        /// 定时执行出队操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ExecuteTask(object sender, System.Timers.ElapsedEventArgs e)
        {
            PersonDequeue();
        }

        /// <summary>
        /// 出队
        /// </summary>
        public static void PersonDequeue()
        {
            if (ConcurrenPersons.Count > 0)
            {
                var dequeueSuccesful = false;
                var peekSuccesful = false;

                Person workItem;

                // Stop
                TimeTask.Instance().Stop();

                peekSuccesful = ConcurrenPersons.TryPeek(out workItem);

                if (peekSuccesful)
                {
                    dequeueSuccesful = ConcurrenPersons.TryDequeue(out workItem); // 出队
                    Console.WriteLine("大家好，我叫" + workItem.Name + "，今年" + workItem.Age + "岁，一大早的就叫老子排队买包子，总算买完了！" +
                                      "        " + DateTime.Now);
                    Thread.Sleep(4000);
                }

                // Start
                TimeTask.Instance().Start();

            }
            else
            {
                Console.WriteLine("队列里没人了，我要关闭定时器啦............");

                // Stop
                TimeTask.Instance().Stop();
            }
        }
    }

}


/*
 *
 * 如果我们换种思路，定时器一秒钟执行一次，但，每次卖包子用时还是五秒，我们应当怎么办？
 * 这样修改的好处时，用户来了，就可以直接买包子，而不用多等五秒，
 * 同理，这一波买包子的人走了后，后续来的人也不需要多等待这个五秒！ 
 */
