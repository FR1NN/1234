using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ConsoleApp22
{
    abstract class Figure : IComparable
    {
        public string Type { get; protected set; }

        public abstract float Square();

        public override string ToString()
        {
            return this.Type + " площадью " + this.Square().ToString();
        }

        public int CompareTo(object obj) //сортировка по площади
        {
            Figure p = (Figure)obj; //приводим к типу figure

            if (this.Square() < p.Square())
                return -1;
            else if (this.Square() == p.Square())
                return 0;
            else
                return 1; //(this.Area() > p.Area())
        }
    }

    class Rectangle : Figure, IPrint
    {
        public float width { get; set; }
        public float height { get; set; }

        public override float Square()
        {
            return width * height;
        }

        public Rectangle(float width, float height)
        {
            this.width = width;
            this.height = height;
            this.Type = "Прямоугольник";
        }

        public void Print()
        {
            Console.WriteLine(this.ToString());
        }
    }

    class Quadre : Rectangle, IPrint
    {
        public Quadre(float width) : base(width, width)
        {
            this.Type = "Квадрат";
        }
    }

    class Circle : Figure, IPrint
    {
        public float r { get; set; }

        public Circle(float r)
        {
            this.r = r;
            this.Type = "Круг";
        }

        public void Print()
        {
            Console.WriteLine(this.ToString()); ;
        }

        public override float Square()
        {
            return 3.1415926535f * r * r;
        }
    }

    interface IPrint
    {
        void Print();
    }

    //Разряженная матрица

    public class Matrix<T>
    {
        Dictionary<string, T> _matrix = new Dictionary<string, T>();
        int maxX;
        int maxY;

        IMatrixCheckEmpty<T> сheckEmpty;

        public Matrix(int px, int py, IMatrixCheckEmpty<T> сheckEmptyParam)
        {
            this.maxX = px;
            this.maxY = py;
            this.сheckEmpty = сheckEmptyParam;
        }

        public T this[int x, int y]
        {
            set
            {
                CheckBounds(x, y);
                string key = DictKey(x, y);
                this._matrix.Add(key, value);
            }
            get
            {
                CheckBounds(x, y);
                string key = DictKey(x, y);
                if (this._matrix.ContainsKey(key))
                {
                    return this._matrix[key];
                }
                else
                {
                    return this.сheckEmpty.getEmptyElement();
                }
            }
        }

        void CheckBounds(int x, int y) //проверка границ
        {
            if (x < 0 || x >= this.maxX)
            {
                throw new ArgumentOutOfRangeException("x", "x=" + x + " выходит за границы");
            }
            if (y < 0 || y >= this.maxY)
            {
                throw new ArgumentOutOfRangeException("y", "y=" + y + " выходит за границы");
            }
        }

        string DictKey(int x, int y) //сборка ключа
        {
            return x.ToString() + "_" + y.ToString();
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            for (int j = 0; j < this.maxY; j++)
            {
                b.Append("[");
                for (int i = 0; i < this.maxX; i++)
                {
                    //Добавление разделителя-табуляции
                    if (i > 0)
                    {
                        b.Append("\t");
                    }
                    //Если текущий элемент не пустой
                    if (!this.сheckEmpty.checkEmptyElement(this[i, j]))
                    {
                        //Добавить приведенный к строке текущий элемент
                        b.Append(this[i, j].ToString());
                    }
                    else
                    {
                        //Иначе добавить признак пустого значения
                        b.Append(" - ");
                    }
                }
                b.Append("]\n");
            }
            return b.ToString();
        }
    }


    public interface IMatrixCheckEmpty<T>
    {
        T getEmptyElement(); //возвращает пустой элемент
        bool checkEmptyElement(T element);
    }

    class FigureMatrixCheckEmpty : IMatrixCheckEmpty<Figure>
    {
        public Figure getEmptyElement() //если пустой элемент
        {
            return null;
        }

        public bool checkEmptyElement(Figure element) //проверка на пустоту
        {
            bool Result = false;
            if (element == null)
            {
                Result = true;
            }
            return Result;
        }
    }


    public class SimpleList<T> : IEnumerable<T> where T : IComparable
    {

        protected SimpleListItem<T> first = null; //первый элемент

        protected SimpleListItem<T> last = null; //последний

        public int Count //подсчет
        {
            get { return _count; }
            protected set { _count = value; }
        }
        int _count;

        public void Add(T element)
        {
            SimpleListItem<T> newItem = new SimpleListItem<T>(element);
            this.Count++;

            if (last == null) //добавение 1 элемента
            {
                this.first = newItem;
                this.last = newItem;
            }
            else //следующих
            {
                //Присоединение элемента к цепочке
                this.last.next = newItem;
                //Присоединенный элемент считается последним
                this.last = newItem;
            }
        }

        //Чтение контейнера с заданным номером

        public SimpleListItem<T> GetItem(int number)
        {
            if ((number < 0) || (number >= this.Count))
            {
                throw new Exception("Выход за границу индекса");
            }
            SimpleListItem<T> current = this.first;
            int i = 0;

            while (i < number)
            {
                //Переход к следующему элементу
                current = current.next;
                //Увеличение счетчика
                i++;
            }
            return current;
        }

        public T Get(int number)
        {
            return GetItem(number).data;
        }

        public IEnumerator<T> GetEnumerator()
        {
            SimpleListItem<T> current = this.first;
            //Перебор элементов
            while (current != null)
            {
                //Возврат текущего значения
                yield return current.data;
                //Переход к следующему элементу
                current = current.next;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Sort() //Cортировка

        {
            Sort(0, this.Count - 1);
        }

        private void Sort(int low, int high)
        {
            int i = low;
            int j = high;
            T x = Get((low + high) / 2);
            do
            {
                while (Get(i).CompareTo(x) < 0) ++i;
                while (Get(j).CompareTo(x) > 0) --j;
                if (i <= j)
                {
                    Swap(i, j);
                    i++; j--;
                }
            } while (i <= j);
            if (low < j) Sort(low, j);
            if (i < high) Sort(i, high);
        }

        private void Swap(int i, int j)
        {
            SimpleListItem<T> ci = GetItem(i);
            SimpleListItem<T> cj = GetItem(j);
            T temp = ci.data;
            ci.data = cj.data;
            cj.data = temp;
        }
    }

    public class SimpleListItem<T>
    {
        public T data { get; set; }

        public SimpleListItem<T> next { get; set; }

        public SimpleListItem(T param)
        {
            this.data = param;
        }
    }

    //Простой стек
    class SimpleStack<T> : SimpleList<T> where T : IComparable
    {
        public void Push(T element) //добаление в стек
        {
            Add(element);
        }

        public T Pop() //чтение с удалением из стека
        {
            T Result = default(T);

            if (this.Count == 0) return Result; //если нет элементов, то возвращаем значение по умолчанию

            if (this.Count == 1) //если один элемент
            {
                Result = this.first.data;
                this.first = null;
                this.last = null;
            }
            else
            {
                SimpleListItem<T> newLast = this.GetItem(this.Count - 2); //ищем последний элемент

                Result = newLast.next.data;

                this.last = newLast; //предполедний приравнивается к последнему

                newLast.next = null;
            }

            this.Count--;

            return Result;
        }
    }




    class Program
    {
        static void Main(string[] args)
        {
            Rectangle rectangle_1 = new Rectangle(4, 6);
            Quadre quadre_1 = new Quadre(8);
            Circle circle_1 = new Circle(2);

            List<Figure> li = new List<Figure>();
            li.Add(rectangle_1);
            li.Add(quadre_1);
            li.Add(circle_1);

            Console.WriteLine("List<Figure>");
            Console.WriteLine("До сортировки:");
            foreach (var x in li) Console.WriteLine(x);
            Console.WriteLine();

            li.Sort();
            Console.WriteLine("После сортировки:");
            foreach (var x in li) Console.WriteLine(x);

            Console.WriteLine("\nМатрица");
            Matrix<Figure> matrix = new Matrix<Figure>(3, 3, new FigureMatrixCheckEmpty());
            matrix[0, 0] = rectangle_1;
            matrix[1, 1] = quadre_1;
            matrix[2, 2] = circle_1;
            Console.WriteLine(matrix.ToString());

            Console.WriteLine("\nСтек");
            SimpleStack<Figure> stack = new SimpleStack<Figure>();

            stack.Push(rectangle_1);
            stack.Push(quadre_1);
            stack.Push(circle_1);

            while (stack.Count > 0)
            {
                Figure f = stack.Pop();
                Console.WriteLine(f);
            }

            ArrayList myAL = new ArrayList();

            myAL.Add(rectangle_1);
            myAL.Add(quadre_1);
            myAL.Add(circle_1);

            Console.WriteLine("\nArrayList");
            Console.WriteLine("\nДо сортировки:");
            PrintValues(myAL);

            myAL.Sort();
            Console.WriteLine("После сортировки:");
            PrintValues(myAL);


            void PrintValues(IEnumerable myList)
            {
                foreach (Object obj in myList)
                    Console.Write("{0}\n", obj);
                Console.WriteLine();
            }

            Console.ReadKey();

        }
    }
}
