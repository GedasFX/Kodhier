using System.Collections.Generic;
using Kodhier.Extensions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Kodhier.Mvc
{
    public class ExecutionResult
    {
        public virtual bool HasInfo => InfoList != null;
        public virtual bool HasWarn => WarnList != null;
        public virtual bool HasError => ErrorList != null;

        public List<string> InfoList { get; set; }
        public List<string> WarnList { get; set; }
        public List<string> ErrorList { get; set; }

        public ExecutionResult AddInfo(string intel)
        {
            if (InfoList == null)
                InfoList = new List<string>();
            InfoList.Add(intel);
            return this;
        }

        public ExecutionResult AddWarn(string intel)
        {
            if (WarnList == null)
                WarnList = new List<string>();
            WarnList.Add(intel);
            return this;
        }

        public ExecutionResult AddError(string intel)
        {
            if (ErrorList == null)
                ErrorList = new List<string>();
            ErrorList.Add(intel);
            return this;
        }

        /// <summary>
        /// Puts the current instance of Execution result into Tempdata
        /// </summary>
        /// <param name="tempData">Temp data to push the ExecutionResult into</param>
        /// <param name="key">Key to put in, optional. If non present will use "ExecutionResult"</param>
        public void PushTo(ITempDataDictionary tempData, string key = null)
        {
            tempData.Put(key ?? "ExecutionResult", this);
        }
    }
}