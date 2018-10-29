using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterSystems.Globals;
using InterSystems.Data.CacheClient;


namespace ConsoleApplication2
{
    class Program
    {
        static Connection Connect()
        {
            //getting conection
            Connection myConn = ConnectionContext.GetConnection();
            //check if the connection is opened
            if (!myConn.IsConnected())
            {
                Console.WriteLine("Connectiong...");
                //if connection is not opened, then connect
                myConn.Connect("User", "_SYSTEM", "SYS");
            }

            if (myConn.IsConnected())
            {
                Console.WriteLine("Successfully connected");
                //if OK then return opened connection
                return myConn;
            }
            else { return null; }
        }

        static void Disconnect(Connection myConn)
        {            
            if (myConn.IsConnected())
                myConn.Close();
        }

        static void Main(string[] args)
        {
            try
            {
                Connection myConn1 = Connect();
                NodeReference nodeRef = myConn1.CreateNodeReference("CardInfo");
                CreateFirstBranch(nodeRef, myConn1);
                CreateSecondBranch(nodeRef, myConn1);
                CreateThirdBranch(nodeRef, myConn1);
                nodeRef.SetSubscriptCount(0);
                ReadData(nodeRef);
                Disconnect(myConn1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }

        static void CreateFirstBranch(NodeReference node, Connection myConn)
        {            
            node.AppendSubscript("111111111111");
            node.Set("Smith,John");
            node.AppendSubscript("DBO546");
            node.Set("Some bank 1");            
            node.AppendSubscript(29244825509100);            
            node.Set(28741.35);            
            node.AppendSubscript(2145632596588547);
            string slip = "Smith,John/1965";
            byte[] bytes = System.Text.Encoding.GetEncoding(1251).GetBytes(slip);
            node.Set(bytes);            
            node.AppendSubscript(1);            
            ValueList myList = myConn.CreateList();
            myList.Append(0, 29244225564111, "John Doe", 500.26, "Payment for goods from ToysRUs");
            node.Set(myList);
            myList.Close();            
            node.SetSubscriptCount(4);            
            node.AppendSubscript(2);            
            myList = myConn.CreateList();
            myList.Append(0, 26032009100100, "John Smith", 115.54, "Transfer to own account in different bank");            
            node.Set(myList);            
            myList.Close();
            Console.WriteLine("Info about first bank is createed");
        }

        static void CreateSecondBranch(NodeReference node, Connection myConn)
        {            
            node.SetSubscriptCount(1);
            node.Set("Some bank 2", "DXO987");
            node.Set(65241.24, "DXO987", 26032009100100);
            string slip = "Smith John||1965";
            byte[] bytes = System.Text.Encoding.GetEncoding(1251).GetBytes(slip);
            node.Set(bytes, "DXO987", 26032009100100, 6541963285249512);            
            ValueList myList = myConn.CreateList();
            myList.Append(1, 29242664509184, "Jane Doe", 500.26, "Recurring payment to Amazon");
            node.Set(myList, "DXO987", 26032009100100, 6541963285249512, 1);
            myList.Close();            
            myList = myConn.CreateList();
            myList.Append(0, 26548962495545, "John Doe", 1015.10, "Payment for delivery");
            node.Set(myList, "DXO987", 26032009100100, 6541963285249512, 2);
            myList.Close();
            Console.WriteLine("Info about second bank is createed");
        }

        static void CreateThirdBranch(NodeReference node, Connection myConn)
        {
            node.SetSubscript(2, "DXJ342");
            node.Set("Some bank 3");
            node.SetSubscript(3, 26008962495545);
            node.Set(126.32);
            node.SetSubscript(4, 4567098712347654);
            string slip = "John Smith 1965";
            byte[] bytes = System.Text.Encoding.GetEncoding(1251).GetBytes(slip);
            node.Set(bytes);
            node.SetSubscript(5, 1);
            ValueList myList = myConn.CreateList();
            myList.Append(0, 29244825509100, "John Smith", 115.54, "Transfer to own account in different bank");
            node.Set(myList);
            myList.Close();
            node.SetSubscript(5, 2);
            myList = myConn.CreateList();
            myList.Append(1, 26032009100100, "John Smith", 1015.54, "Transfer to own account in different bank");
            node.Set(myList);
            myList.Close();
            Console.WriteLine("Info about third bank is createed");
        }

        static void ReadData(NodeReference node)
        {
            try
            {                
                node.AppendSubscript("");             
                string subscr = node.NextSubscript();                
                while (!subscr.Equals(""))
                {
                    node.SetSubscript(node.GetSubscriptCount(), subscr);
                    if (node.HasData())
                    {
                        Console.Write(" ".PadLeft(node.GetSubscriptCount() * 4, '-') + subscr + "  ->  ");
                        GetData(node);
                    }
                    if (node.HasSubnodes())
                    {
                        ReadData(node);
                    }
                    subscr = node.NextSubscript();
                }
            }
            catch (GlobalsException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                node.SetSubscriptCount(node.GetSubscriptCount() - 1);
            }
        }

        static void GetData(NodeReference node)
        {
            Object value = node.GetObject();
            if (value is string)
            {
                if ((node.GetSubscriptCount() == 1) || (node.GetSubscriptCount() == 2))
                {
                    Console.WriteLine(value.ToString());
                }
                else if (node.GetSubscriptCount() == 5)
                {
                    ValueList outList = node.GetList();
                    outList.ResetToFirst();

                    for (int i = 0; i < outList.Length - 1; i++)
                    {
                        Console.Write(outList.GetNextObject() + ", ");
                    }
                    Console.WriteLine(outList.GetNextObject());
                    outList.Close();
                }
                else if (node.GetSubscriptCount() == 4)
                {
                    string tempString = Encoding.GetEncoding(1251).GetString(node.GetBytes());
                    Console.WriteLine(tempString);
                }
            }
            else if (value is double)
            {
                Console.WriteLine(value.ToString());
            }
            else if (value is int)
            {
                Console.WriteLine(value.ToString());
            }
        }
    }
}
