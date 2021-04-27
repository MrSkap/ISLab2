using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ISLab2
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			chart1.Series[0].Points.Clear();
			chart1.Series[1].Points.Clear();
			chart1.Series[2].Points.Clear();
			FillMass(0.6, 0.8, dataGridView1,0);
			Random random = new Random();
			//Console.Write("Введите a:");
			//double readA = Convert.ToDouble(Console.ReadLine());
			double readA = Convert.ToDouble(textBoxA.Text);
			//Console.Write("Введите b:");
			//double readB = Convert.ToDouble(Console.ReadLine());
			double readB = Convert.ToDouble(textBoxB.Text);
			//Console.WriteLine();
			for (int i = 0; i < Interval.points.Length; i++)
			{
				Interval.points[i] = new Point(Interval.x1 + i, readA * Math.Exp(readB * (Interval.x1 + i)));
			}
			for (int i = 0; i < Interval.points.Length; i++)
			{
				if (Interval.points[i].Y > Interval.MaxY)
				{
					Interval.MaxY = Interval.points[i].Y;
				}
				if (Interval.points[i].Y < Interval.MinY)
				{
					Interval.MinY = Interval.points[i].Y;
				}
			}
			Population population1 = new Population();
			for (int i = 0; i < population1.persons.Length; i++)
			{
				double a = random.NextDouble();
				double b = random.NextDouble();
				population1.persons[i] = new Person(Interval.Min, Interval.Max, a, b);
			}
			population1.Alg(Convert.ToDouble(textBoxFitness1.Text));
			var result1 = population1.GetMaxFitness();
			labelA1.Text = Convert.ToString(result1.A);
			labelB1.Text = Convert.ToString(result1.B);
			FillMass(result1.A, result1.B, dataGridView2,1);

			Population population2 = new Population();
			for (int i = 0; i < population2.persons.Length; i++)
			{
				double a = random.NextDouble();
				double b = random.NextDouble();
				population2.persons[i] = new Person(Interval.Min, Interval.Max, a, b);
			}
			population2.Alg(Convert.ToDouble(textBoxFitness2.Text));
			var result2 = population2.GetMaxFitness();
			labelA2.Text = Convert.ToString(result2.A);
			labelB2.Text = Convert.ToString(result2.B);
			FillMass(result2.A, result2.B, dataGridView3,2);


			
		}


		void FillMass(double A, double B, DataGridView dataGridView, int seriesNumber)
		{
			for(int x = 0; x < 10; x++)
			{
				double y = A * Math.Exp(B * x);
				dataGridView.Rows.Add(x, y);
				chart1.Series[seriesNumber].Points.AddXY(x, y);
			}
		}
	}
}
