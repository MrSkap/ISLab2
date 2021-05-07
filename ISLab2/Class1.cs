using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime;

namespace ISLab2
{
	class Point2 //класс точек
	{
		public double X;
		public double Y;
		public Point2(double x, double y)
		{
			X = x;
			Y = y;
		}
	}

	class Person2 // класс особей
	{
		public double A;
		public double B;
		public double GenA;
		public double GenB;
		public double Fitness; // приспособленность
		// конструктор, принимающий декодированные параметры и переводящий их в закодированные
		public Person2(double a, double b)
		{
			A = a;
			GenA = new Random().NextDouble();
			B = b;
			GenB = new Random().NextDouble();
			SetFitness();
		}
		public Person2(double[] a, double[] b)
		{
			A = a;
			GenA = new Random().NextDouble();
			B = b;
			GenB = new Random().NextDouble();
			SetFitness();
		}
		// вычисление приспособленности
		public void SetFitness()
		{
			double Sum = 0;
			Point2[] pointsPerson = new Point2[Interval2.x2 - Interval2.x1 + 1];
			for (int i = 0; i < pointsPerson.Length; i++)
			{//функция 
				pointsPerson[i] = new Point2(Interval2.x1 + i, A * Math.Exp(B * (Interval2.x1 + i)));
				Sum += Math.Pow(Interval.points[i].Y - pointsPerson[i].Y, 2);
			}
			// чем меньше значение, тем больше приспособлена особь
			Fitness = Math.Sqrt(Sum / pointsPerson.Length) / Math.Abs(Interval2.MaxY - Interval2.MinY);
		}
		// "склеивание" 2 генов в хромосому
		public double[] GetChromosome()
		{
			double alfa = new Random().NextDouble();
			//double newChromosome1 = alfa * GenA + (1 - alfa) * GenB;
			//double newChromosome2 = (1-alfa) * GenA + alfa * GenB;
			double[] genMass = new double[2];
			genMass[0] = GenA;
			genMass[1] = GenB;
			return genMass;
		}

	}

	class Interval2
	{
		public const int GenLength = 10;
		public const double Min = 0; // концы интервала значений a и b
		public const double Max = 1;
		public static int x1 = 0; // концы интервала значений x
		public static int x2 = 9;
		public static double MinY = 100000;
		public static double MaxY = 0;
		public static Point2[] points = new Point2[x2 - x1 + 1];
		public static double getDecodedGenValue(double gen)
		{
			int[] array = new int[1];
			gen.CopyTo(array, 0);
			return (array[0] * (Max - Min)) / (Math.Pow(2, GenLength) - 1) + Min;
		}
	}


	class Population2
	{
		static int npersons = 20;
		// 20 особей в популяции
		public Person2[] persons;
		public Population2()
		{
			persons = new Person2[npersons];
		}
		// выбор особи с максимальной приспособленностью
		public Person2 GetMaxFitness() // макисмальная приспособленность соответсвует минимальному значению Fitness
		{
			double MinFitnessValue = persons[0].Fitness;
			int PersonIndex = 0;
			for (int i = 1; i < persons.Length; i++)
			{
				if (persons[i].Fitness < MinFitnessValue)
				{
					PersonIndex = i;
					MinFitnessValue = persons[PersonIndex].Fitness;
				}
			}
			return persons[PersonIndex];
		}
		// алгоритм
		public void Alg()
		{
			Random random = new Random();
			// условие останова можно варьировать
			while (GetMaxFitness().Fitness > 0.005)
			{
				Person2[] parents = new Person2[persons.Length];
				// выбор двух случайных особей из популяции,
				// отбор одной из них для селекции
				for (int i = 0; i < parents.Length; i++)
				{
					int Index1 = random.Next(npersons);
					var competitor1 = persons[Index1];
					int Index2 = random.Next(npersons);
					while (Index2 == Index1)
					{
						Index2 = random.Next(npersons);
					}
					var competitor2 = persons[Index2];
					if (competitor1.Fitness > competitor2.Fitness)
					{
						parents[i] = competitor2;
					}
					else
					{
						parents[i] = competitor1;
					}
				}
				// для каждой пары последовательно идущих особей
				for (int i = 0; i < parents.Length; i += 2)
				{//!!!!!!!!!!!!
					double[] chromosome1 = parents[i].GetChromosome();
					double[] chromosome2 = parents[i + 1].GetChromosome();
					// случайное определение точки разрыва для кроссинговера


					int CrossoverPoint = random.Next(chromosome1.Length - 1) + 1;
					double[] child1 = new double[2];
					double[] child2 = new double[2];
					// первая часть хромосомы первого родителя передаётся
					// в первую часть хромосомы первого потомка,
					// первая часть второго - второму
					double alfa = new Random().NextDouble();
					//double newChromosome1 = alfa * GenA + (1 - alfa) * GenB;
					//double newChromosome2 = (1-alfa) * GenA + alfa * GenB;
					child1[0] = alfa * chromosome1[0] + (1 - alfa) * chromosome2[1];
					child2[1] = alfa * chromosome2[0] + (1 - alfa) * chromosome1[1];
					// вторая часть хромосомы второго родителя передаётся
					// первому потомку,
					// вторая часть хромосомы первого родителя - второму потомку
					for (int k = CrossoverPoint; k < Interval.GenLength * 2; k++)
					{
						child1[0] = alfa * chromosome1[0] + (1 - alfa) * chromosome2[1];
						child2[1] = alfa * chromosome2[0] + (1 - alfa) * chromosome1[1];
						// проводим мутацию
						Mutation(child1);
						Mutation(child2);
					}
					BitArray GenAChild1 = new BitArray(Interval.GenLength);
					BitArray GenBChild1 = new BitArray(Interval.GenLength);
					BitArray GenAChild2 = new BitArray(Interval.GenLength);
					BitArray GenBChild2 = new BitArray(Interval.GenLength);
					// разбиваем хромосомы каждого потомка на два гена
					for (int y = 0; y < Interval.GenLength; y++)
					{
						GenAChild1.Set(y, child1[y]);
						GenBChild1.Set(y, child1[y + Interval.GenLength]);
						GenAChild2.Set(y, child2[y]);
						GenBChild2.Set(y, child2[y + Interval.GenLength]);
					}
					// заменяем в популяции родителей потомками
					persons[i] = new Person2(GenAChild1, GenBChild1);
					persons[i + 1] = new Person2(GenAChild2, GenBChild2);
				}
			}
		}

		public void Alg(double UserFitness)
		{
			Random random = new Random();
			// условие останова можно варьировать
			while (GetMaxFitness().Fitness > UserFitness)
			{
				Person2[] parents = new Person2[persons.Length];
				// выбор двух случайных особей из популяции,
				// отбор одной из них для селекции
				for (int i = 0; i < parents.Length; i++)
				{
					int Index1 = random.Next(npersons);
					var competitor1 = persons[Index1];
					int Index2 = random.Next(npersons);
					while (Index2 == Index1)
					{
						Index2 = random.Next(npersons);
					}
					var competitor2 = persons[Index2];
					if (competitor1.Fitness > competitor2.Fitness)
					{
						parents[i] = competitor2;
					}
					else
					{
						parents[i] = competitor1;
					}
				}
				// для каждой пары последовательно идущих особей
				for (int i = 0; i < parents.Length; i += 2)
				{
					double[] chromosome1 = parents[i].GetChromosome();
					double[] chromosome2 = parents[i + 1].GetChromosome();
					// случайное определение точки разрыва для кроссинговера
					int CrossoverPoint = random.Next(chromosome1.Length - 1) + 1;
					double[] child1 = new double[2];
					double[] child2 = new double[2];
					// первая часть хромосомы первого родителя передаётся
					// в первую часть хромосомы первого потомка,
					// первая часть второго - второму
					for (int j = 0; j < CrossoverPoint; j++)
					{
						child1.Set(j, chromosome1[j]);
						child2.Set(j, chromosome2[j]);
					}
					// вторая часть хромосомы второго родителя передаётся
					// первому потомку,
					// вторая часть хромосомы первого родителя - второму потомку
					for (int k = CrossoverPoint; k < Interval.GenLength * 2; k++)
					{
						child1.Set(k, chromosome2[k]);
						child2.Set(k, chromosome1[k]);
						// проводим мутацию
						Mutation(child1);
						Mutation(child2);
					}
					BitArray GenAChild1 = new BitArray(Interval2.GenLength);
					BitArray GenBChild1 = new BitArray(Interval2.GenLength);
					BitArray GenAChild2 = new BitArray(Interval2.GenLength);
					BitArray GenBChild2 = new BitArray(Interval2.GenLength);
					// разбиваем хромосомы каждого потомка на два гена
					for (int y = 0; y < Interval2.GenLength; y++)
					{
						GenAChild1.Set(y, child1[y]);
						GenBChild1.Set(y, child1[y + Interval2.GenLength]);
						GenAChild2.Set(y, child2[y]);
						GenBChild2.Set(y, child2[y + Interval2.GenLength]);
					}
					// заменяем в популяции родителей потомками
					persons[i] = new Person2(GenAChild1, GenBChild1);
					persons[i + 1] = new Person2(GenAChild2, GenBChild2);
				}
			}
		}
		// мутация
		public void Mutation(double[] child)
		{
			Random random = new Random();
			double Pm = 0.05; // 20^(-1) = 5/100 = 0,05
			if (Pm > new Random(1).NextDouble())
			{
				child[0] += new Random(9).NextDouble();
			}
			if (Pm > new Random(2).NextDouble())
			{
				child[1] += new Random(3).NextDouble();
			}
		}
	}
}
