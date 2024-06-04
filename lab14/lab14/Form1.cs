using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab14
{
    public partial class Form1 : Form
    {
        private List<Operator> operators = new List<Operator>();
        private Queue<Client> clientQueue = new Queue<Client>();
        private Random random = new Random();
        DataTable table = new DataTable();
        private int totalClients = 0;

        public Form1()
        {
            InitializeComponent();
            dataGridView1.DataSource = table;
            table.Columns.Add("Operator", typeof(string));
            table.Columns.Add("Client", typeof(string));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int numberOfOperators = int.Parse(numericUpDown1.Text);

            operators.Clear();

            for (int i = 0; i < numberOfOperators; i++)
            {
                operators.Add(new Operator { IsAvailable = true, Name = "op_" + i.ToString() });
            }

            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (random.NextDouble() < 0.5) // 50% шанс добавить клиента каждые пол секунды
            {
                clientQueue.Enqueue(new Client { ServiceTime = random.Next(1, 10), Name = "cl_" + random.Next(100, 1000).ToString() });
            }

            totalClients += clientQueue.Count;
            label3.Text = totalClients.ToString();

            // Проверка доступных операторов и назначение клиентов
            foreach (var op in operators)
            {
                if (op.IsAvailable && clientQueue.Count > 0)
                {
                    var client = clientQueue.Dequeue();
                    op.IsAvailable = false;

                    DataRow newRow = table.NewRow();
                    newRow["Operator"] = op.Name;
                    newRow["Client"] = client.Name;
                    table.Rows.Add(newRow);

                    // Запланировать время, когда оператор снова будет доступен
                    Task.Delay(client.ServiceTime * 500).ContinueWith(t => op.IsAvailable = true);
                }
            }
        }
    }
}

public class Client
{
    public int ServiceTime { get; set; }

    public string Name { get; set; }
}

public class Operator
{
    public bool IsAvailable { get; set; }

    public string Name { get; set; }
}
