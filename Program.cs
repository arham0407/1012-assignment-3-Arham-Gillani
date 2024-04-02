
const int maxDataSize = 31;
int dataSize = 0;
string[] dataInFile;
string[] dates = new string[maxDataSize];
double[] salesAmnt = new double[maxDataSize];

string userChoice = "";
string fileName = "";
string filePath = "";

bool isNotValid = true;
//Console.Clear();

loadMenu();
while(isNotValid) {
  try {
    
    userChoice = Prompt("\nEnter a Main Menu Choice ('M' to display menu): ").ToUpper();

    if(userChoice == "C") {
      createFile();
    } else if(userChoice == "L") {
      dataSize = loadData();
    } else if(userChoice == "S") {
      saveData();
    } else if(userChoice == "D") {
      displayData();
    } else if(userChoice == "A") {
      dataSize = AddData();
    } else if(userChoice == "E") {
      editData();
    } else if(userChoice == "M") {
      loadMenu();
    } else if(userChoice == "R") {

      while(true) {
        if(dataSize == 0) {
          throw new Exception("No entries loaded from {fileName}. Please load a file into memory");
        } else {
          loadSubMenu();
          string userSubChoice = Prompt("\nEnter an Analysis Menu Choice: ").ToUpper();
          if(userSubChoice == "A") {
            double salesAve = salesAmnt.Sum()/ dataSize;
            Console.WriteLine($"Average amount in sales is: {salesAve:c2}");
          } else if(userSubChoice == "H") {
            double highestSales = salesAmnt.Max();
            Console.WriteLine($"Highest amount in sales is: {highestSales:c2}");
          } else if(userSubChoice == "L") {
            double lowestSales = salesAmnt.Min();
            Console.WriteLine($"Lowest amount in sales is: {lowestSales:c2}");
          } else if(userSubChoice == "G") {
            createGraph();
          } else if(userSubChoice == "R") {
            throw new Exception("Returning to Main Menu");
          }
        }
      }

    } else if (userChoice == "Q") {
      isNotValid = false;
      throw new Exception("Thank you for using this application. Come back anytime.");
    }


  } catch (Exception ex) {
    Console.WriteLine(ex.Message);
  }
}



void loadMenu() {
  Console.WriteLine("\n========== MENU OPTIONS ==========");  
	Console.WriteLine("[C] Create a new file");
	Console.WriteLine("[L] Load Values from File to Memory");
	Console.WriteLine("[S] Save Values from Memory to File");
	Console.WriteLine("[D] Display Values in Memory");
	Console.WriteLine("[A] Add Value in Memory");
	Console.WriteLine("[E] Edit Value in Memory");
	Console.WriteLine("[R] Analysis Menu");
  Console.WriteLine("[M] Display Main Menu");
	Console.WriteLine("[Q] Quit");
}

void loadSubMenu() {
  Console.WriteLine("===== ANALYSIS MENU OPTIONS ====="); 
  Console.WriteLine($"[A] Get Average of Values in Memory");
	Console.WriteLine($"[H] Get Highest Value in Memory");
	Console.WriteLine($"[L] Get Lowest Value in Memory");
	Console.WriteLine($"[G] Graph Values in Memory");
	Console.WriteLine($"[R] Return to Main Menu");
}


string GetFileName()
{
  DirectoryInfo dirInfo = new DirectoryInfo(@"./data");
  FileInfo[] files = dirInfo.GetFiles();
  
	string fileName = "";
	do {
    try {
      int fileIndex = 0;
      Console.WriteLine("\nList of Files: ");
      foreach(FileInfo file in files) {
        Console.WriteLine($"[{fileIndex}] {file.Name}");
        fileIndex += 1;
      }
      Console.Write("Choose File to load: ");
      fileIndex = int.Parse(Console.ReadLine().Trim());
      
      fileName = files[fileIndex].Name;
      break;
    } catch(Exception ex) {
      Console.WriteLine(ex.Message);
    }
    
	} while (string.IsNullOrWhiteSpace(fileName));
	return fileName;
}


void createFile() {
  try {
    Console.WriteLine($"Enter name of file: ");
    string myFile = Console.ReadLine();
    fileName = myFile + ".csv";
    filePath = $"data/" + fileName;
    string[] dataLine = ["Entry Date, Amount"];
    File.WriteAllLines(filePath, dataLine);
    Console.WriteLine($"New file successfully created at: {Path.GetFullPath(fileName)}");
  } catch(Exception ex) {
    Console.WriteLine(ex.Message);
  }
}

int loadData() {
  fileName = GetFileName();
  filePath = $"data/{fileName}";
  dataSize = 0;
  dataInFile = File.ReadAllLines(filePath);
  
  for(int i = 0; i < dataInFile.Length; i++) {
    string[] items = dataInFile[i].Split(',');
    if(i != 0) {
      dates[dataSize] = items[0];
      salesAmnt[dataSize] = double.Parse(items[1]);
      dataSize++;
    }
  }

  Console.WriteLine($"\nLoad complete. {fileName} has {dataSize} data entries");
  return dataSize;
}

void saveData() {
  dataSize++;
  filePath = $"data/{fileName}";
  string[] items = new string[dataSize];
  int itemIndex = 0;
  items[0] = "Entry Date, Amount";
  for(int i = 1; i < dataSize; i++) {
    items[i] = $"{dates[itemIndex]}, {salesAmnt[itemIndex]}";
    itemIndex++;
  }
  File.WriteAllLines(filePath,items);
  Console.WriteLine($"All Data successfully written to file at: {Path.GetFullPath(filePath)}");
}

void displayData() {
  if(dataSize == 0) {
    throw new Exception($"No Entries loaded from {fileName}. Please load a file to memory or add a value in memory");
  } else {
    Console.Clear();
    Console.WriteLine($"\nCurrent Loaded Entries:  {dataSize}\n");
    Console.WriteLine("{0,4} {1,-15} {2,10:}\n", "[#]", "Entry Date", "Amount");
    for (int i = 0; i < dataSize; i++) {
      Console.WriteLine("{0,4} {1,-15} {2,10:f2}", "["+i+"]", dates[i], salesAmnt[i]);
    }
  }
}

int AddData() {
  if(dataSize >= 31) {
    Console.WriteLine($"Data is already full. Can't add anymore entry");
  } else {
    Console.WriteLine($"Number of Data: {dataSize}");
    string inputDate = checkData("date");
    string inputSales = checkData("amount");

    dates[dataSize] = inputDate;
    salesAmnt[dataSize] = double.Parse(inputSales);
    dataSize++;

    Console.WriteLine($"\nSuccessfully added to temporary memory. \n{inputDate}, {inputSales:c2}");
  }
  
  return dataSize;
}

void editData() {
  if(dataSize == 0) {
    throw new Exception($"No Entries loaded from {fileName}. Please load a file to memory or add a value in memory");
  } else {
    displayData();
    while(true) {
      try { 
        Console.Write($"Choose index of data to edit [0-{dataSize-1}]: ");
        int dataIndex = int.Parse(Console.ReadLine().Trim());
        if(dataIndex >=0 && dataIndex < dataSize) {
          Console.WriteLine($"\nYou are editing this data: \n{dates[dataIndex],-15} {salesAmnt[dataIndex], 10:c2}");

          Console.Write($"Enter Amount of Sales: (0-1000): ");
          double inputSales = double.Parse(Console.ReadLine());
          salesAmnt[dataIndex] = inputSales;
          break;
        }
      } catch (Exception ex) {
        Console.WriteLine(ex.Message);
      }
    }
    Console.WriteLine($"Successfully updated data.");
  }
  
}


void createGraph() {
  Console.WriteLine($"=== Sales of the month of {fileName} ===");

  Console.WriteLine($"Dollars");
  Array.Sort(dates, salesAmnt, 0, dataSize);

  int dollars = 1000;
  string perLine = "";

  while(dollars >= 0 ) {
    Console.Write($"{dollars, 4}|");
    // for(int i = 0; i < dataSize; i++) {
    //   string[] salesDay = dates[i].Split('-');
      
    //   if(salesAmnt[i] >= dollars && salesAmnt[i] <= (dollars + 49)) {
    //     perLine += $"{salesAmnt[i], 7:f2}";
    //     break;
    //   } else {
    //     perLine += $"{' ', 7:n2}";
    //   }
    // }
    string[] salesDay = dates[0].Split('-');

    for(int i = 1; i <= maxDataSize; i++) {
      string formatDay = i.ToString("00");
      int dayIndex = Array.IndexOf(dates, $"{salesDay[0]}-{formatDay}-{salesDay[2]}"); 

      if(dayIndex != -1) {
        if(salesAmnt[dayIndex] >= dollars && salesAmnt[dayIndex] <= (dollars + 49)) {
          perLine += $"{salesAmnt[dayIndex], 7:f2}";
          break;
        } else {
          perLine += $"{' ', 5}";
        }
      } else {
        perLine += $"{' ', 5}";
      }  
    }
    Console.WriteLine($"{perLine}");
    perLine = "";
    dollars -= 50;
  }

  string line = "-----";
  string days = "";

  for(int i = 1; i <= maxDataSize; i++) {
    string formatDay = i.ToString("00");
    line += "----";
    days += $"{formatDay, 5}";
  }

  // for(int i = 0; i < dataSize; i++) {
  //   string[] salesDay = dates[i].Split('-');
  //   line += "-------";
  //   days += $"{salesDay[1], 7}";
  // }
  Console.WriteLine($"{line}");
  Console.Write($"Date|");
  Console.Write($"{days}");


  Console.WriteLine();
  
}


string Prompt(string prompt) {
  string response = "";
  Console.Write(prompt);
  response = Console.ReadLine().Trim();
  return response;
}

string checkData(string dataType) {
  string myData = "";
  while(true) {
    try {
      if(dataType == "date") {
        
        Console.Write($"Enter Date of Sales: (MM-dd-YYYY): ");
        myData = Console.ReadLine();
        if(dates.Contains(myData)) {
          Console.WriteLine($"Data already exist on this date. Please enter another date or choose Edit Data");
        } else {
          break;
        }
      } else if(dataType == "amount") {

        Console.Write($"Enter Amount of Sales: (0-1000): ");
        myData = Console.ReadLine();

        if(double.Parse(myData) < 0 || double.Parse(myData) > 1000) {
          Console.WriteLine($"Invalid sales amount. Must be between 0 and 1000");
        } else {
          break;
        }
      }

    } catch (Exception ex) {
      Console.WriteLine(ex.Message);
    }

  }
  
  return myData;
}

