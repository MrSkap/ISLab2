using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ISLab2
{
	class Point //класс точек
	{
		public double X;
		public double Y;
		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}
	}

	class Person // класс особей
	{
		public double A;
		public double B;
		public BitArray GenA;
		public BitArray GenB;
		public double Fitness; // приспособленность
	    // конструктор, принимающий декодированные параметры и переводящий их в закодированные
		public Person(double min, double max, double a, double b)
		{
			A = a;
			int EncodedA = (int)Math.Round((a - min) * (Math.Pow(2, Interval.GenLength) - 1) / (max - min));
			GenA = new BitArray(new int[] { EncodedA });
			GenA.Length = Interval.GenLength;
			B = b;
			int EncodedB = (int)Math.Round((b - min) * (Math.Pow(2, Interval.GenLength) - 1) / (max - min));
			GenB = new BitArray(new int[] { EncodedB });
			GenB.Length = Interval.GenLength;
			SetFitness();
		}
		// конструктор, принимающий закодированные параметры и переводящий их в декодированные

		public Person(BitArray genA, BitArray genB)
		{
			A = Interval.getDecodedGenValue(genA);
			B = Interval.getDecodedGenValue(genB);
			GenA = genA;
			GenB = genB;
			SetFitness();
		}
		// вычисление приспособленности
		public void SetFitness()
		{
			double Sum = 0;
			Point[] pointsPerson = new Point[Interval.x2 - Interval.x1 + 1];
			for (int i = 0; i < pointsPerson.Length; i++)
			{//функция 
				pointsPerson[i] = new Point(Interval.x1 + i, A * Math.Exp(B * (Interval.x1 + i)));
				Sum += Math.Pow(Interval.points[i].Y - pointsPerson[i].Y, 2);
			}
			// чем меньше значение, тем больше приспособлена особь
			Fitness = Math.Sqrt(Sum / pointsPerson.Length) / Math.Abs(Interval.MaxY  - Interval.MinY);
		}
		// "склеивание" 2 генов в хромосому
		public BitArray GetChromosome()
		{
			bool[] chromosome = new bool[Interval.GenLength * 2];
			GenA.CopyTo(chromosome, 0);
			GenB.CopyTo(chromosome, Interval.GenLength);
			return new BitArray(chromosome);
		}

	}

	class Interval
	{
		public const int GenLength = 10;
		public const double Min = 0; // концы интервала значений a и b
		public const double Max = 1;
		public static int x1 = 0; // концы интервала значений x
		public static int x2 = 9;
		public static double MinY = 100000;
		public static double MaxY = 0;
		public static Point[] points = new Point[x2 - x1 + 1];
		public static double getDecodedGenValue(BitArray gen)
		{
			int[] array = new int[1];
			gen.CopyTo(array, 0);
			return (array[0] * (Max - Min)) / (Math.Pow(2, GenLength) - 1) + Min;
		}
	}


	class Population
	{
		static int npersons = 20;
		// 20 особей в популяции
		public Person[] persons;
		public Population()
		{
			persons = new Person[npersons];
		}
		// выбор особи с максимальной приспособленностью
		public Person GetMaxFitness() // макисмальная приспособленность соответсвует минимальному значению Fitness
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
				Person[] parents = new Person[persons.Length];
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
					BitArray chromosome1 = parents[i].GetChromosome();
					BitArray chromosome2 = parents[i + 1].GetChromosome();
					// случайное определение точки разрыва для кроссинговера
					int CrossoverPoint = random.Next(chromosome1.Length - 1) + 1;
					BitArray child1 = new BitArray(Interval.GenLength * 2);
					BitArray child2 = new BitArray(Interval.GenLength * 2);
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
					persons[i] = new Person(GenAChild1, GenBChild1);
					persons[i + 1] = new Person(GenAChild2, GenBChild2);
				}
			}
		}

		public void Alg(double UserFitness)
		{
			Random random = new Random();
			// условие останова можно варьировать
			while (GetMaxFitness().Fitness > UserFitness)
			{
				Person[] parents = new Person[persons.Length];
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
					BitArray chromosome1 = parents[i].GetChromosome();
					BitArray chromosome2 = parents[i + 1].GetChromosome();
					// случайное определение точки разрыва для кроссинговера
					int CrossoverPoint = random.Next(chromosome1.Length - 1) + 1;
					BitArray child1 = new BitArray(Interval.GenLength * 2);
					BitArray child2 = new BitArray(Interval.GenLength * 2);
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
					persons[i] = new Person(GenAChild1, GenBChild1);
					persons[i + 1] = new Person(GenAChild2, GenBChild2);
				}
			}
		}
		// мутация
		public void Mutation(BitArray child)
		{
			Random random = new Random();
			double Pm = Math.Pow(child.Length, -1); // 20^(-1) = 5/100 = 0,05
			for (int i = 0; i < Interval.GenLength * 2; i++)
			{
				double rnd = random.Next(100) / 100;
				if (Pm > rnd)
				{
					child.Set(i, !child[i]);
				}
			}
		}
	}
}
