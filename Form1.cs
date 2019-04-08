using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RE2NFAv2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        internal static int Prec(char ch)
        {
            switch (ch)
            {
                case '.': return 1;
                case '*': return  3;
                case '|':
                case '+': return  2;
            }
            return -1;
        } 

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            String exp = textBox1.Text;
           // char[] arr = exp.ToCharArray();
            //Array.Reverse(arr);
            //exp = new String(arr);
            string result = "";

            // initializing empty stack  
            Stack<char> stack = new Stack<char>();

            for (int i = 0; i < exp.Length; ++i)
            {
                char c = exp[i];

                // If the scanned character is an operand, add it to output.  
                if (char.IsLetterOrDigit(c))
                {
                    result += c;
                }

                // If the scanned character is an '(', push it to the stack.  
                else if (c == '(')
                {
                    stack.Push(c);
                }

                //  If the scanned character is an ')', pop and output from the stack   
                // until an '(' is encountered.  
                else if (c == ')')
                {
                    while (stack.Count > 0 && stack.Peek() != '(')
                    {
                        result += stack.Pop();
                    }

                    if (stack.Count > 0 && stack.Peek() != '(')
                    {
                       // return "Invalid Expression"; // invalid expression 
                    }
                    else
                    {
                        stack.Pop();
                    }
                }
                else // an operator is encountered 
                {
                    while (stack.Count > 0 && Prec(c) <= Prec(stack.Peek()))
                    {
                        result += stack.Pop();
                    }
                    stack.Push(c);
                }

            }

            // pop all the operators from the stack  
            while (stack.Count > 0)
            {
                result += stack.Pop();
            }
            //arr = result.ToCharArray();
            //Array.Reverse(arr);
            //result = new String(arr);
            //MessageBox.Show(result);
            create_graph(result);
        }
        void create_graph(String s)
        {
                Stack<nfa> eval = new Stack<nfa>();
            int d = 0;
            foreach(char c in s)
            {
                if(char.IsLetterOrDigit(c))
                {
                    String[] arr1 = { "S" + d++, "S" + d++ };
                    //char[] arr = { c };
                    nfa n = new nfa();
                    n.start=n.add_state(arr1[0]);
                    n.end=n.add_state(arr1[1]);
                    n.add_connect(n.start, n.end, c);
                    //n.table[0, 0] = "1";
                    
                    eval.Push(n);
                }
                if (c == '.')
                {
                    nfa n1 = eval.Pop();
                    nfa n2 = eval.Pop();
                    nfa n3 = new nfa();
                    //n2.end = n1.end;
                    foreach(state i in n2.states)
                    {
                        if (i.name != n2.end.name)
                        {
                            state dd = n3.add_state(i.name);
                            dd.trans = i.trans;
                            dd.eps = i.eps;
                        }
                    }

                    foreach (state i in n1.states)
                    {
                        if(i.name!=n1.start.name)
                        {
                            state dd = n3.add_state(i.name);
                            dd.trans = i.trans;
                            dd.eps = i.eps;
                        }
                    }
                    n3.states.Remove(n1.start);
                    //n3.states.Remove(n2.end);
                    n2.end.eps = n1.start.eps;
                    n2.end.trans = n1.start.trans;
                    n3.states.Add(n2.end);
                    //n3.add_connect(n2.end, n1.start, 'E');
                    n3.start = n2.start;
                    n3.end = n1.end;
                        
                    eval.Push(n3);   
                }
                if (c == '*')
                {
                    nfa n1 = eval.Pop();
                    //new states to be added
                    state s1 = new state("S"+d++);
                    state s2 = new state("S"+d++);
                    nfa n2 = new nfa();
                    n2.start = s1;
                    n2.end = s2;
                    n2.states.Add(n2.start);
                    //n2.add_state(s1.name);
                    foreach (state i in n1.states)
                    {
                        state dd=n2.add_state(i.name);
                        dd.eps = i.eps;
                        dd.trans = i.trans;
                    }
                    n2.add_state(s2.name);


                    //n2.start.eps.Add(n2.end);
                    //n2.start.eps.Add(y.start);
                    //n2.states.Remove(n2.start);
                    //n2.states.Add(n2.start);
  

                    n2.add_connect(n2.start, n1.start, 'E');
                    n2.add_connect(n2.start, n2.end, 'E');
                    n2.add_connect(n1.end, n1.start, 'E');
                    n2.add_connect(n1.end, n2.end, 'E');
                    eval.Push(n2);
 
                }
                if (c == '+')
                {
                    //a+b*
                    nfa x = eval.Pop();
                    nfa y = eval.Pop();
                    state s1 = new state("S" + d++);
                    state s2 = new state("S" + d++);
                    nfa n2 = new nfa();
                    //MessageBox.Show(s1.name + "_" + s2.name);
                    n2.end = s2;
                    
                    //n2.add_state(s2.name);
                    n2.start = s1;
                    x.end.eps.Add(n2.end);
                    String p = x.end.name + " " + x.end.eps[0].name;
                    //MessageBox.Show(p);
                    y.end.eps.Add(n2.end);
                    //y.states.Remove(y.end);
                    
                    //n2.states.Add(n2.end);
                    //MessageBox.Show(x.end.name+"__"+y.end.name);
                    foreach (state i in x.states)
                    {
                        if (i.name != x.end.name)
                        {
                            state dd = n2.add_state(i.name);
                            dd.eps = i.eps;
                            dd.trans = i.trans;
                        }
                        else {
                            state dd = n2.add_state(x.end.name);
                            dd.eps = x.end.eps;
                            dd.trans = x.end.trans;
                        }

                    }
                    //x.end.eps.Add(n2.end);
                    //y.end.eps.Add(n2.end);
                    foreach (state i in y.states)
                    {
                        if (i.name != y.end.name)
                        {
                            state dd = n2.add_state(i.name);
                            dd.eps = i.eps;
                            dd.trans = i.trans;
                        }
                        else
                        {
                            state dd = n2.add_state(i.name);
                            dd.eps = y.end.eps;
                            dd.trans = y.end.trans;
                        }
                    }
                    //n2.add_state(s2.name);
                    //MessageBox.Show(n2.start.name + "_" + x.start.name + "_" + y.start.name);
                    n2.start.eps.Add(x.start);
                    n2.start.eps.Add(y.start);
                    n2.states.Remove(n2.start);
                    n2.states.Add(n2.start);
                    n2.states.Add(n2.end);
                    //n2.states.Remove(n2.start);
                    //n2.states.Add(n2.end);
                    //n2.add_connect(n2.start, x.start, 'E');
                    //n2.add_connect(n2.start, y.start, 'E');
                    //MessageBox.Show(n2.end.name + "_" + x.end.name + "_" + y.end.name);
                    //n2.add_connect( x.end, n2.end,'E');
                    //n2.add_connect( y.end, n2.end,'E');
                    
                    
                    
                    
                    eval.Push(n2);
                }

            }
            nfa final = eval.Pop();
            traverse(final);
            convert(final);
            //after parsing the string add by yourself a final and initial state
        }

        public void convert(nfa n)
        {
 
        }

        public void traverse(nfa n)
        {
           // var comparer = new Comparer 
           // n.states.Sort();
           // IComparer<state> a= new sComparer();
           // n.states.Sort(a);

            richTextBox1.Text = ""; 
            foreach(state i in n.states)
            {
                richTextBox1.Text += "State " + i.name+"\n";
                foreach (char c in i.trans.Keys)
                {
                    richTextBox1.Text += "d(" + i.name + "," +c.ToString()+")->"+ i.trans[c].name+"\n";
                }
                richTextBox1.Text += "e-closure : ";
                foreach (state j in i.eps)
                {
                    richTextBox1.Text += j.name + " , ";
                }
                richTextBox1.Text += "\n_______________\n";
            }
            richTextBox1.Text += "\nInitial State: " + n.start.name + "\nFinal State: " + n.end.name;
            
            List<state> dfa1 = eps_close(n.start);
            dfa1 = new HashSet<state>(dfa1).ToList<state>();
            String ans="";
            foreach (state x in dfa1)
                ans += x.name + "_";
            //MessageBox.Show(ans);
            
            convert_dfa(n);
        }
        public void convert_dfa(nfa n)
        {
            dfa d1 = new dfa();
            List<state> dfst = new HashSet<state>(eps_close(n.start)).ToList<state>();
            d1.start = dfst;
            bool a = d1.states.Add(d1.start);
            //bool b = d1.states.Add(eps_close(n.start));
            SortedSet<char> symbols = new SortedSet<char>();
            foreach (state i in n.states)
            {

                foreach (char c in i.trans.Keys)
                    symbols.Add(c);
                   
            }/*
            String ans = "";
            foreach (char x in symbols.ToList<char>())
                ans += x + "_";
            MessageBox.Show(ans);*/
            //MessageBox.Show(a.ToString() + "__" + b.ToString());
            if (dfst.Contains(n.end))
            {
                d1.end.Add(dfst);
            }
            String x = "";
            //int ctr = 0;
            //bool b = true;
            List<state> loop= new List<state>();
            //::::PROBLEM IDENTIFIED REMEMBER TO PASS ALL GIVEN STATES THROUGH IT do not update dfst inside foreach loop like dat::::
            //noe recursion noe stack
            Stack<List<state>> steck= new Stack<List<state>>();
            steck.Push(dfst);
            while ( steck.Count>0)
            {
                
              //  bool r = false;
                foreach (char c in symbols)
                {
                    String l = STR(dfst);
                    loop = newdfastate(c,dfst);
                    foreach(var d in loop.ToList())
                    {
                        loop.AddRange(eps_close(d));
                    }
                    loop= new HashSet<state>(loop).ToList<state>();
                    richTextBox2.Text += "d( "+l + " , " + c + " )->" + STR(loop)+"\n";
                   
                    if (loop.Contains(n.end))
                     {
                         d1.end.Add(loop);
                     }
                     bool ch= d1.states.Add(loop);
                    
                    if (ch)
                     {
                         steck.Push(loop);
                        // foreach (var k in dfst)
                        //     richTextBox2.Text += k.name + "_";
                      //   richTextBox2.Text += "\n";
                     }
                //    r = r | ch;
                }
                dfst = steck.Pop();
                richTextBox2.Text += "_________\n";
               //b=b&r;
            }
           // String x="";
            //richTextBox2.Text = x;
            //MessageBox.Show(d1.states.Count.ToString());
           
            
            
            
            x = "\t";
            String[] states = new String[d1.states.Count];
            String[,] graph= new String[states.Length,states.Length];
            for (int g1 = 0; g1 < states.Length; g1++)
            {
                for (int g2 = 0; g2 < states.Length; g2++)
                {
                    graph[g1, g2] = "_";
                }
            }
            String file = "";
            int j = 0;
            foreach (var st in d1.states)
            {
                states[j++] = STR(st);
                x += states[j - 1] + "\t";
                file += j.ToString() + ",";
            }
            var rem = new List<state>();
            file += "\n";
            x += "\n";
            
            int ind1=0, ind2;
            dfst = d1.start;
            bool flag = true;
            foreach (    char c in symbols )
            {
                flag = true;
                foreach (var v in d1.states)
                {
                    
                    rem = v;
                    dfst = newdfastate(c, v);
                    foreach (var d in dfst.ToList())
                    {
                        dfst.AddRange(eps_close(d));
                    }
                    dfst = new HashSet<state>(dfst).ToList<state>();
                    ind2 = Array.IndexOf(states, STR(dfst));
                    if (ind2 == -1)
                    {
                        flag = false;
                        //MessageBox.Show(STR(dfst));
                    }
                    else
                    {
                        if (ind1 != ind2)
                        { graph[ind1, ind2] += c.ToString(); }
                        else if (flag == true) graph[ind1, ind2] += c.ToString();
                        ind1 = ind2;//er jonne last er sobai * hoe jache;
                    }
                }
                
            }
            
            //MessageBox.Show(STR(newdfastate('b', rem)) + "__" + STR(rem));
            for (int g1 = 0; g1 < states.Length; g1++)
            {
                x += states[g1] + "\t";
                //file += g1.ToString()+",";
                for (int g2 = 0; g2 < states.Length; g2++)
                {
                    file += graph[g1, g2] + ",";
                    x += graph[g1, g2] + "\t";
                }
                file += "\n";
                x += "\n";
                //file_and_png(graph,file);
            }
            file_and_png(graph, file);
            // richTextBox2.Text+="\n"+x;
            richTextBox2.Text += "\nSTART STATE: " + STR(d1.start);
            richTextBox2.Text += "\nEND STATE(S):\n";
            HashSet<String> endz = new HashSet<string>();
            foreach(var t in d1.end)
            {
                endz.Add(STR(t));
            }
            //d1.end =new HashSet<List<state>>(d1.end).ToList<List<state>>();
            foreach (var p in endz)
            {
                richTextBox2.Text += p;
                richTextBox2.Text += "____";
            }
          //  file_and_png(graph,file);
            
        }
        String STR(List<state> st)
        {
            String res="";
            foreach (var s in st)
                res += s.name.Substring(1);
            res = res == "" ? res : "S" + res;
            return res;
        }
        void file_and_png(String[,] file,String f)
        {

            System.IO.File.WriteAllText(@"graph.csv",f);
            //Form2 s = new Form2()

            List<String> nodes = new List<string>();
            for (int i = 0; i < file.GetLength(0); i++)
            {
                nodes.Add("S"+i);
            }
            drawnodes(nodes,file);
        }
        void drawnodes(List<String> N,String[,] graph)
        {
            //label1.Text = (N.Count()).ToString();
            // int n = 7;
            //// Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            // Graphics g = Graphics.FromImage(bmp);
            // //g.TranslateTransform(pictureBox1.Location.X,pictureBox1.Location.Y);
            //  g.FillRectangle(Brushes.Black,new Rectangle(pictureBox1.Width/2-10,pictureBox1.Height/2-10,20,20));
            //  pictureBox1.Image = bmp;
            CalculateVertices(N.Count, 300, 0, new Point(490,300), N,graph);

        }
        private void CalculateVertices(int sides, int radius, int startingAngle, Point center, List<String> N,String[,] graph)
        {
            // if (sides < 3)
            //    throw new ArgumentException("Polygon must have 3 sides or more.");

            List<Point> points = new List<Point>();
            float step = 360.0f / sides;

            float angle = startingAngle; //starting angle
            for (double i = startingAngle; i < startingAngle + 360.0; i += step) //go in a full circle
            {
                points.Add(DegreesToXY(angle, radius, center)); //code snippet from above
                angle += step;
            }
            Bitmap bmp = new Bitmap(1378, 780);
            Graphics g = Graphics.FromImage(bmp);
            //g.TranslateTransform(pictureBox1.Location.X,pictureBox1.Location.Y);
            //  g.FillRectangle(Brushes.Black, new Rectangle(center.X - 10, center.Y - 10, 25, 25));
            // pictureBox1.Image = bmp;
            int k = 0;
            foreach (Point x in points)
            {
                //g.FillEllipse(Brushes.Firebrick, new Rectangle(x, new Size(15, 15)));
                Pen p1 = new Pen(Brushes.MediumVioletRed);
                //  p1= Pens.MediumVioletRed;
                //float f = (float)(3.0);
                p1.Width = 3.0F;
                //g.DrawLine(p1, new Point(x.X + 20, x.Y + 15), new Point(center.X, center.Y));
                Font fon2 = new System.Drawing.Font("Times New Roman", 18.0f);
                
                
                g.FillEllipse(Brushes.GreenYellow, new Rectangle(x, new Size(40, 30)));
                Pen p2 = new Pen(Brushes.Firebrick); p2.Width = 2.5F;
                g.DrawEllipse(p2, new Rectangle(x, new Size(40, 30)));
                try
                {
                    if (k < N.Count)
                        g.DrawString(N.ElementAt(k++), fon2, Brushes.DodgerBlue, new PointF(x.X, x.Y - 20));
                }
                catch (Exception ll) { MessageBox.Show(ll.ToString() + "\n******\n" + (N.Count()).ToString() + "***" + (k - 1).ToString()); }
            }
            Pen ppp = new Pen(Brushes.MediumVioletRed);
            Font fon3 = new System.Drawing.Font("Times New Roman", 18.0f);
            ppp.Width = 4.0F;
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                for (int j = 0; j < graph.GetLength(0); j++)
                {
                    if (graph[i, j] != "_")
                    {
                        if (i != j)
                        {
                            g.DrawLine(ppp, points[i], points[j]);
                            //Point dir=new Point((points[i].X + points[j].X)/3, (points[i].Y + points[j].Y)/3);
                            g.DrawRectangle(ppp,points[i].X, points[i].Y, 10, 10);
                            g.DrawString(graph[i,j], fon3, Brushes.Black, new Point((points[i].X + points[j].X)/2, (points[i].Y + points[j].Y)/2));
                        }
                        if (i == j)
                        {
                            g.DrawEllipse(ppp, new Rectangle(points[i].X , points[i].Y , 80, 80));
                            g.DrawString(graph[i,j], fon3, Brushes.DodgerBlue, new Point(points[i].X+80,points[i].Y+80));
                        }
                    }
                }
            }
            //g.FillEllipse(Brushes.Black, new Rectangle(center.X - 25, center.Y - 25, 50, 50));
            //g.DrawString("Sink", new Font("Arial Bold", 25.0f), Brushes.ForestGreen, new PointF(center.X - 35, center.Y - 55));
            Form2 f = new Form2();
            f.BackgroundImage = bmp;
            f.Show();
            //.Image = bmp;
            // return points.ToArray();
        }
        private Point DegreesToXY(float degrees, float radius, Point origin)
        {
            Point xy = new Point();
            double radians = degrees * Math.PI / 180.0;

            xy.X = (int)(Math.Cos(radians) * radius + origin.X);
            xy.Y = (int)(Math.Sin(-radians) * radius + origin.Y);

            return xy;
        }
        List<state> newdfastate(char symbol, List<state> df)
        {
            SortedSet<state> trans = new SortedSet<state>(new stateComparer());
            foreach (state s in df)
            {
                try {
                    trans.Add(s.trans[symbol]);}
                catch(Exception err){}
            }
            return trans.ToList<state>();
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
           // this.Height = 1500;
        }

        List<state> eps_close(state s)
        {
            List<state> closure = new List<state>();
            closure.Add(s);
            if (s.eps.Count == 0)
                return closure;
            else
            {
                foreach (state i in s.eps)
                {
                    closure.Add(i);
                    closure.AddRange(eps_close(i));
                }
            }
            return closure;
        }
    }

    public class state
    {
        public String name;
        public Dictionary<Char,state> trans;// = new Dictionary<Char,state>();
        public List<state> eps;// = new List<state>();
        public state(String a)
        {
            name = a; // next_state = null;
            trans=  new Dictionary<Char,state>();
            eps = new List<state>(); 

        }
        public void add_trans(state ns, char ip)
        {
            if (ip != 'E')
            {
                trans[ip] = ns;
            }
            else
            {
                eps.Add(ns);
            }
       }
    }
    public class dfa
    {
        public List<state> start;
        public List<List<state>> end = new List<List<state>>();//member is_end in state class would have sufficed. Waste of Space
        public SortedSet<List<state>> states = new SortedSet<List<state>>(new sComparer());

    }
    public class nfa
    {
        public state start;
        public state end;
        public List<state> states= new List<state>();
        
        public state add_state(String s_name)
        {
            state a = new state(s_name);
            states.Add(a);
            return a;
            //a.add_trans()
            //states.Add(new state(s_name, ip));
        }
        //public state add_state(state s)
        //{ 
        //}
        public void add_connect(state a, state b, char ip)
        {
            
            a.add_trans(b, ip);

        }
        //public List<String> states = new List<string>();
        //public List<char> inputs = new List<char>();
        //public String[,] table;//= new String
        //public Dictionary<String, String> table = new Dictionary<string, string>();
       /* public nfa(String[] st, char[] ip)
        {
            states.AddRange(st);
            inputs.AddRange(ip);
            //table = //new String[states.Count, inputs.Count];
            //rules in program itself;
        }
        public nfa(List<String> st, List<char> ip)
        {
            states.AddRange(st);
            inputs.AddRange(ip);
            //table = new String[states.Count, inputs.Count];
            //for (int i = 0; i < states.Count; i++)
            //{
              //  for (int j = 0; j < inputs.Count; j++)
               // {
               //     table[i, j] = "_";
                //}
            //}
            //rules in program i
        }/*
        public nfa connect(nfa x)
        { 

        }*/
    }
    public class sComparer : System.Collections.Generic.IComparer<List<state>>
    {
        public int Compare(List<state> ob1, List<state> ob2)
        {
            /*if (transition1.Key == transition2.Key)
                return transition1.Value.CompareTo(transition2.Value);
            else
                return transition1.Key.CompareTo(transition2.Key);*/
           
                foreach (state s in ob1)
                {
                    if (ob2.Contains(s) == false)
                        return 1;
                }
                //return 0;
            
            return 0;
        }
       
    }
    public class stateComparer : System.Collections.Generic.IComparer<state>
    {
        public int Compare(state s1, state s2)
        {
            if (s1.name.CompareTo(s2.name) == 0)
                return 0;
            else
                return 1;
        }
    }
}
