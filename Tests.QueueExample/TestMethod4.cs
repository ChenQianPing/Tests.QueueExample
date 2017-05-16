using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.QueueExample
{
    class TestMethod4
    {
        public static ConcurrentQueue<Person> ConcurrenPersons = new ConcurrentQueue<Person>();

        // 赋值为false也就是没有信号，Mark by:CHENQP
        public static readonly AutoResetEvent MyResetEvent = new AutoResetEvent(false);

        public void MockTest()
        {
            Console.WriteLine("店小二：都别吵，都别吵，再等五秒钟开始卖包子。5 4 3 2 1 ...");
            var thread = new Thread(PersonDequeue) {Name = "queue"};
            thread.Start();

            // 模拟入队
            var model1 = new Person("宋江", "男", 66);
            var model2 = new Person("李逵", "男", 53);

            var listPerson = new List<Person> {model1, model2};

            // 开始排队
            foreach (var item in listPerson)
            {
                PersonEnqueue(item);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// 入队
        /// </summary>
        public static void PersonEnqueue(Person model)
        {
            ConcurrenPersons.Enqueue(model);

            // 这里是设置为有信号，Mark by:CHENQP
            MyResetEvent.Set();

            Thread.Sleep(2000);
        }

        /// <summary>
        /// 出队
        /// </summary>
        public static void PersonDequeue()
        {
            while (true)
            {
                // 执行到这个地方时，会等待set调用后改变了信号才接着执行，Mark by：CHENQP
                MyResetEvent.WaitOne();

                if (ConcurrenPersons.Count > 0)
                {
                    var peekSuccesful = false;

                    Person workItem;

                    peekSuccesful = ConcurrenPersons.TryPeek(out workItem);

                    if (!peekSuccesful) continue;
                    ConcurrenPersons.TryDequeue(out workItem); // 出队
                    Console.WriteLine("大家好，我叫" + workItem.Name + "，今年" + workItem.Age + "岁，一大早的就叫老子排队买包子，总算买完了！" +
                                      "        " + DateTime.Now);
                }
                else
                {
                    Console.WriteLine("队列里没人了............");
                }

            }
        }



    }
}


/*
 * 如果我们不使用定时器，该怎么写呢？
 * 不使用定时器时，我们可以使用C#的信号量机制.
 * 
 * 简单来说只有调用Set（）方法后才能执行WaitOne（）后面的代码，
 * AutoResetEvent和ManualResetEvent分别都有Set()改变为有信号 ,
 * Reset()改变为无信号，
 * WaitOne()将会阻塞当前调用的线程，直到有信号为止，
 * 即执行了Set（）方法，WaitOne()方法还可以带指定时间的参数.
 */
