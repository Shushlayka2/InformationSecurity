#include <iostream>
#include <list>

using namespace std;

void Inicialize();
void Precomputation();
list<int> SieveOfEratosthenes(int n);
bool IsCorrect(int &p);
int EuclideanAlgorithm(int a, int b);
int ExtendedEuclidsAlgorithm(int a, int b);
int FindCorrectA(int x);
int FirstTrialDivision();
void MainLoop();
int SecondTrialDivision(int g);

int u, v, k, below;
int *G, *I, *A;
list<int> P;

int main()
{	
 	Inicialize();

	Precomputation();

	int g = FirstTrialDivision();

	MainLoop();

	g = SecondTrialDivision(g);

	cout << g << endl;

	system("pause");
	return 0;
}

void Inicialize()
{
	cout << "Sorenson's alghotitm for finding k - ary GDC(u, v)" << endl << "Pass necessary values:" << endl;
	cout << "u = "; cin >> u;
	cout << "v = "; cin >> v;
	cout << "k = "; cin >> k;
	below = (int)floor(sqrt(k) + 1);
}

void Precomputation()
{
	P = SieveOfEratosthenes(k);
	P.remove_if(IsCorrect);

	G = new int[k];
	for (int i = 0; i < k; i++)
		G[i] = EuclideanAlgorithm(i, k);

	I = new int[k];
	for (int i = 0; i < k; i++)
	{
		if (G[i] == 1)
			I[i] = ExtendedEuclidsAlgorithm(k, i);
	}

	A = new int[k];
	for (int i = 0; i < k; i++)
	{
		if (G[i] == 1)
			A[i] = FindCorrectA(i);
	}
}

list<int> SieveOfEratosthenes(int n)
{
	bool *A = new bool[n + 1];
	for (int i = 0; i < n + 1; i++)
		A[i] = true;

	int count = 0, i = 2;
	while (i * i <= n)
	{
		if (A[i] == true)
		{
			int j = i * i;
			while (j <= n)
			{
				A[j] = false;
				j += i;
			}
		}
		i++;
	}

	list<int> answer;
	for (int i = 2; i <= n; i++)
		if (A[i] == true)
			answer.push_back(i);
	delete[] A;
	return answer;
}

bool IsCorrect(int &p)
{
	return { (p > below) && (k % p != 0)};
}

int EuclideanAlgorithm(int a, int b)
{
	while ((a != 0) && (b != 0))
		if (a > b)
			a = a % b;
		else
			b = b % a;
	return a + b;
}

list<int> GetListOfDivs(int a, int b)
{
	list<int> result;
	int remainder = a % b;
	int div;
	while (remainder != 0)
	{
		div = a / b;
		result.push_back(div);
		a = b;
		b = remainder;
		remainder = a % b;
	}
	return result;
}

int ExtendedEuclidsAlgorithm(int a, int b)
{
	list<int> divs = GetListOfDivs(a, b);
	divs.reverse();
	int x = 0, y = 1;
	for each (int div in divs)
	{
		int temporary = x;
		x = y;
		y = temporary - (y * div);
	}
	if (y < 0)
		y = a + y;
	return y;
}

int FindCorrectA(int x)
{
	int  min = k, b, bestresult = 0;
	for (int a = 1; a <= sqrt(k); a++)
	{
		b = -1 * a * x;
		if (abs(b) > sqrt(k))
			b = k + b;
		if (abs(a) + abs(b) < min)
		{
			bestresult = a;
			min = a + b;
		}
	}
	return bestresult;
}

int FirstTrialDivision()
{
	int g = 1;
	for each (int d in P)
	{
		while ((u % d == 0) && (v % d == 0))
		{
			u = u / d;
			v = v / d;
			g = g * d;
		}
	}
	return g;
}

void MainLoop()
{
	int t, tempu, tempv, x, a, b;
	while ((u != 0) && (v != 0))
	{
		tempu = u % k;
		tempv = v % k;
		if (G[tempu] > 1)
			u = u / G[tempu];
		else
			if (G[tempv] > 1)
				v = v / G[tempv];
			else
			{
				x = (tempu * I[tempv]) % k;
				a = A[x];
				b = (-1 * a * x) % k;
				if (abs(b) > sqrt(k))
					b = b + k;
				t = abs(a * u + b * v) / k;
				if (u > v)
					u = t;
				else
					v = t;
			}
	}
}

int SecondTrialDivision(int g)
{
	int t;
	if (v == 0)
		t = u;
	else
		t = v;
	for each(int d in P)
	{
		while (t % d == 0)
			t = t / d;
	}
	return t * g;
}