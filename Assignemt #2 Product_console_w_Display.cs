// See https://aka.ms/new-console-template for more information


using System;
using System.Data;
using Npgsql;

class Sample
{
    static void Main(string[] args)
    {
        // Connect to a PostgreSQL database
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1:5432;User Id=postgres; " +
           "Password=Silverboot9;Database=prods;");
        conn.Open();

        // Define a query returning a single row result set
        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM product", conn);
        NpgsqlCommand customertable = new NpgsqlCommand("SELECT * FROM customer", conn);

        // Execute the query and obtain the value of the first column of the first row
        //Int64 count = (Int64)command.ExecuteScalar();
        NpgsqlDataReader reader = command.ExecuteReader();
        DataTable product_table = new DataTable();
        product_table.Load(reader);

        NpgsqlDataReader reads = customertable.ExecuteReader();
        DataTable customer_table = new DataTable();
        customer_table.Load(reads);

        DataTable dt = new DataTable();
        dt.Load(reader);
        problem_7(product_table);
        print_results(dt);
        problem_20(customer_table); 

    }

    static void print_results(DataTable data)
    {
        Console.WriteLine();
        Dictionary<string, int> colWidths = new Dictionary<string, int>();

        foreach (DataColumn col in data.Columns)
        {
            Console.Write(col.ColumnName);
            var maxLabelSize = data.Rows.OfType<DataRow>()
                    .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                    .OrderByDescending(m => m).FirstOrDefault();

            colWidths.Add(col.ColumnName, maxLabelSize);
            for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 14; i++) Console.Write(" ");
        }

        Console.WriteLine();

        foreach (DataRow dataRow in data.Rows)
        {
            for (int j = 0; j < dataRow.ItemArray.Length; j++)
            {
                Console.Write(dataRow.ItemArray[j]);
                for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 14; i++) Console.Write(" ");
            }
            Console.WriteLine();
        }
    }
    static void problem_7(DataTable data)

    {
        string filter = "prod_quantity > 12 and prod_quantity< 30";
        DataRow[] filterows = data.Select(filter);

        DataTable filteredDataTable = data.Clone();
        foreach (DataRow row in filterows)
        {
            filteredDataTable.ImportRow(row);
        }
        print_results(filteredDataTable);

    }

    static void problem_20(DataTable data)
    {
        Console.WriteLine("problem 20");

        var result = data.AsEnumerable()
            .GroupBy(r =>
            {
                int rep_id;
                int.TryParse(r.Field<string>("rep_id"), out rep_id);
                return rep_id;

            })
            .Select(g => new
            {
                rep_id = g.Key,
                Total_balance = g.Sum(r => r.Field<decimal>("cust_balance"))

            });
        var filteredResult = result.Where(r => r.Total_balance > 12000 && r.rep_id> 0)
                                    .OrderBy(result => result.rep_id);
    
        foreach (var results in filteredResult) 
        {
            Console.WriteLine("Rep ID: {0}, Total Balance: {1}", results.rep_id, results.Total_balance);
        }
    }

}





