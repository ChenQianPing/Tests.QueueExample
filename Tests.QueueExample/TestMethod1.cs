using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.QueueExample
{
    public class TestMethod1
    {
        public static ConcurrentQueue<Person> ConcurrenPersons = new ConcurrentQueue<Person>();

        public void MockTest()
        {
            // 模拟入队
            var model1 = new Person("宋江", "男", 66);
            var model2 = new Person("李逵", "男", 53);
            var model3 = new Person("顾大嫂", "女", 46);
            var model4 = new Person("扈三娘", "女", 46);
            var model5 = new Person("一丈青", "女", 36);
            var model6 = new Person("林冲", "男", 45);
            var model7 = new Person("武松", "男", 42);
            var model8 = new Person("花和尚", "男", 39);
            var listPerson = new List<Person> {model1, model2, model3, model4, model5, model6, model7, model8};

            // 开始排队
            foreach (var item in listPerson)
            {
                PersonEnqueue(item);
            }
            // 排队完成

            // 注册Timer 在web项目中可以在 ApplicationStart 或者 静态构造函数中注册
            TimeTask.Instance().ExecuteTask += new System.Timers.ElapsedEventHandler(ExecuteTask);
            // 表示间隔,5秒钟执行一次
            TimeTask.Instance().Interval = 1000*5; 
            TimeTask.Instance().Start();

            Console.WriteLine("店小二：都别吵，都别吵，再等五秒钟开始卖包子。5 4 3 2 1 ...");
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

                peekSuccesful = ConcurrenPersons.TryPeek(out workItem);

                if (peekSuccesful)
                {
                    dequeueSuccesful = ConcurrenPersons.TryDequeue(out workItem); // 出队
                    Console.WriteLine("大家好，我叫" + workItem.Name + "，今年" + workItem.Age + "岁，一大早的就叫老子排队买包子，总算买完了！" +
                                      "        " + DateTime.Now);
                }
            }
            else
            {
                Console.WriteLine("队列里没人了............");
            }
        }
    }

    public class TimeTask
    {
        public event System.Timers.ElapsedEventHandler ExecuteTask;

        private static readonly TimeTask Task = null;
        private System.Timers.Timer _timer = null;

        // 定义时间
        public int Interval { set; get; } = 1000*5;

        static TimeTask()
        {
            Task = new TimeTask();
        }

        public static TimeTask Instance()
        {
            return Task;
        }

        // 开始
        public void Start()
        {
            if (_timer != null) return;
            _timer = new System.Timers.Timer(Interval);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            _timer.Enabled = true;
            _timer.Start();
        }

        protected void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ExecuteTask?.Invoke(sender, e);
        }

        // 停止
        public void Stop()
        {
            if (_timer == null) return;
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

    }

}


/*
 * 
 * 本篇示例 讲解C#队列 入队  和  定时出队.
 * 例如：早上排队买包子 设置为每隔五秒，买包子成功排队的人出队！
 */
