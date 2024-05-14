using SeatingChart.Data;
using SeatingChart.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using SeatingChart;
using System.ComponentModel;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace SeatingChart.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly ChartContext _context;
        private readonly IConfiguration Configuration;

        public IndexModel(ChartContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }
        public int ChartNum {get;set;}


        public List<Student> Undergrads { get; set; }
        public List<Student> Grads { get; set; }
        public List<Other> Others { get; set; }
        public List<Name> Names { get; set; }

        public String [] DisplayNames { get; set; }

        public int numCols { get; set;}

        public async Task OnGetAsync(int chartNum)
        {
            ChartNum = chartNum;


            IQueryable<Student> undergradsIQ = (from s in _context.Students
                                             where !s.isGrad
                                             select s).OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ThenBy(s => s.MiddleName);
            IQueryable<Student> gradsIQ = (from s in _context.Students
                                             where s.isGrad
                                             select s).OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ThenBy(s => s.MiddleName);
            
            IQueryable<Other> othersIQ = (from o in _context.Others
                                            select o).OrderBy(o => o.Order);   

        
            var conf = from c in _context.Configurations.Where(c => c.ID == ChartNum) select c;
            numCols = 2;
            if(await conf.AnyAsync()) {
                numCols = (await conf.FirstAsync()).NumberofColumns;
            }

            // var pageSize = Configuration.GetValue("PageSize", 4);
            // Students = await PaginatedList<Student>.CreateAsync(
                // studentsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
            Undergrads = undergradsIQ.ToList();
            Grads = gradsIQ.ToList();
            Others = othersIQ.ToList();
            Names = getNames(Undergrads, Grads, Others, numCols);
            DisplayNames = getDisplayNames(Names);
        }

        // Method to turn list of grads, undergrads, and others into one array of names
        private List<Name> getNames(List<Student> underGrads, List<Student> grads, List<Other> others, int cols){
            List<Name> nameList = new List<Name>();
            int gradIndex = 0;
            int otherIndex = 0;
            // This loop should never run because all grads should fit on the same row, ie. there should be more columns than grads, but it is not necassarily a requirnment
            for(int i = 0; i < grads.Count - (grads.Count % cols); i++){
                nameList.Add(new Name(grads[i].FirstName, grads[i].MiddleName, grads[i].LastName, 1, grads[i].ID));
                gradIndex = i;
            }

            // put the grad students in the middle of the row with others on either side of them
            for(int i = 0; i < cols; i++){
                if(i > (cols - grads.Count)/2 && gradIndex < grads.Count){
                    nameList.Add(new Name(grads[gradIndex].FirstName, grads[gradIndex].MiddleName, grads[gradIndex].LastName, 1, grads[gradIndex].ID));
                    gradIndex ++;
                }
                else if(otherIndex < others.Count){
                    nameList.Add(new Name(others[otherIndex].FirstName, others[otherIndex].MiddleName, others[otherIndex].LastName, 2, others[otherIndex].ID));
                    otherIndex ++;
                }
                else{
                    nameList.Add(new Name());
                }
            }

            // Add all undergrads
            foreach(Student underGrad in underGrads){
                nameList.Add(new Name(underGrad.FirstName, underGrad.MiddleName, underGrad.LastName, 0, underGrad.ID));
            }

            // Fill in the seats on the last row of undergrads with empty names
            for(int diff = cols - (nameList.Count % cols); diff > 0; diff--){
                nameList.Add(new Name());
            }

            // Add the rest of the other names
            for(; otherIndex < others.Count; otherIndex++){
                nameList.Add(new Name(others[otherIndex].FirstName, others[otherIndex].MiddleName, others[otherIndex].LastName, 2, others[otherIndex].ID));
            }

            // Fill in the rest of the chart with empty names
            for(int diff = cols - (nameList.Count % cols); diff > 0 ; diff--){
                nameList.Add(new Name());
            }
            return nameList;
        }

        private String [] getDisplayNames (List<Name> nms){
            Name [] names = nms.ToArray();
            Dictionary<String, List<int>> nameDic = new Dictionary<string, List<int>>();
            String [] displayNames = (from s in names select s.LastName).ToArray();
            for (int i = 0; i < names.Length; i++){
                if(!nameDic.ContainsKey(names[i].LastName)){
                    nameDic.Add(names[i].LastName, new List<int>());
                }
                nameDic[names[i].LastName].Add(i);
            }
            HashSet<int> c = new HashSet<int>();
            for (int i = 0; i < names.Length; i++){
                if(!c.Contains(i)){
                    if(nameDic[displayNames[i]].Count > 1){
                        DisplayHelper(new Dictionary<String, List<int>>{{displayNames[i] , nameDic[displayNames[i]]}}, displayNames, names, 0);
                        foreach(int j in nameDic[names[i].LastName]){
                            c.Add(j);
                        }
                    }
                }
            }
            return displayNames;
        }
        private void DisplayHelper (Dictionary<String, List<int>> dispDic, String[] displayNames, Name [] names, int swch){
            Dictionary<String, List<int>> newDispDic = new Dictionary<string, List<int>>();
            foreach(String s in dispDic.Keys){
                foreach(int i in dispDic[s]){
                    switch(swch){
                        case 0:
                            displayNames[i] = names[i].FirstName.Length >0 ? $"{names[i].FirstName.Substring(0,1)} {displayNames[i]}" : "";
                            break;
                        case 1:
                            if(names[i].MiddleName != null && names[i].MiddleName.Length > 0){
                                displayNames[i] = $"{names[i].FirstName.Substring(0,1)}.{names[i].MiddleName.Substring(0,1)}. {names[i].LastName}";
                            }
                            break;
                        case 2:
                            if(names[i].MiddleName != null && names[i].MiddleName.Length > 0){
                                displayNames[i] = $"{names[i].FirstName} {names[i].MiddleName.Substring(0,1)} {names[i].LastName}";
                            }else{
                                displayNames[i] = $"{names[i].FirstName} {names[i].LastName}";
                            }
                            break;
                        case 3:
                            if(names[i].MiddleName != null && names[i].MiddleName.Length > 0){
                                displayNames[i] = $"{names[i].FirstName} {names[i].MiddleName} {names[i].LastName}";
                            }
                            break;
                        default:
                            return;
                    }
                    if(!newDispDic.ContainsKey(displayNames[i])){
                        newDispDic.Add(displayNames[i], new List<int>());
                    }
                    newDispDic[displayNames[i]].Add(i);
                }
            }
            List<String> toRemove = new List<String>();
            foreach(String s in newDispDic.Keys){
                if(!(newDispDic[s].Count > 1)){
                    toRemove.Add(s);
                }
            }
            foreach(String s in toRemove){
                newDispDic.Remove(s);
            }
            if(newDispDic.Keys.Count > 0){
                DisplayHelper(newDispDic, displayNames, names, swch + 1);
            }
        }
    }
    public class Name{
        public String FirstName {get;set;}
        public String MiddleName {get;set;}
        public String LastName {get;set;}
        // Type 0 = Undergrad; Type 1 = Grad; Type 2 = Other
        public int Type {get;set;}
        public int ID {get;set;}

        public Name(String FirstName = "", String MiddleName = "", String LastName = "", int Type = 2, int ID = 0){
            this.FirstName = FirstName;
            this.MiddleName = MiddleName;
            this.LastName = LastName;
            this.Type = Type;
            this.ID = ID;
        }
    }
}
