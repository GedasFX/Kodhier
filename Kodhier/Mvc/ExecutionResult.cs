using System;
using System.Collections.Generic;
using System.Linq;

namespace Kodhier.Mvc
{
    public class ExecutionResult
    {
        public bool HasInfo => InfoList != null;
        public bool HasWarn => WarnList != null;
        public bool HasError => ErrorList != null;

        private List<string> InfoList { get; set; }
        private List<string> WarnList { get; set; }
        private List<string> ErrorList { get; set; }

        public void AddInfo(string intel)
        {
            if (InfoList == null)
                InfoList = new List<string>();
            InfoList.Add(intel);
        }

        public void AddWarn(string intel)
        {
            if (WarnList == null)
                WarnList = new List<string>();
            WarnList.Add(intel);
        }

        public void AddError(string intel)
        {
            if (ErrorList == null)
                ErrorList = new List<string>();
            ErrorList.Add(intel);
        }

        public IEnumerable<string> EnumerateInfo() => InfoList?.AsEnumerable();
        public IEnumerable<string> EnumerateWarm() => WarnList?.AsEnumerable();
        public IEnumerable<string> EnumerateError() => ErrorList?.AsEnumerable();
    }
}