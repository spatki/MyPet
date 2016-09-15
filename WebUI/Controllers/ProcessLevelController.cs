using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Filters;
using ProcessAccelerator.WebUI.Mappers;
using ProcessAccelerator.WebUI.BAL.AccessControl;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class ProcessLevelController : Cruder<mstr_process_level, mstr_process_levelInput>
    {

        public ProcessLevelController(ICrudService<mstr_process_level> service, IMapper<mstr_process_level, mstr_process_levelInput> v, IWorkflowService wf)
            : base(service, v, wf, "DFPRSLVL")
        
        {
            functionID = "DFPRSLVL";
        }

        protected override string RowViewName
        {
            get { return "GetItems"; }
        }

        protected override string listDisplayName(mstr_process_level o) { return o.LongName; }

        protected override bool checkForDuplication(mstr_process_levelInput input) 
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true; 
            else return false;
        }

        protected override bool checkForDuplicateEdit(mstr_process_levelInput input)
        {
            var entity = service.Where(rec => rec.ClientID == ((PAIdentity)User.Identity).clientID && rec.ID != input.ID && rec.ShortName.Trim().Equals(input.ShortName.Trim()));
            if (entity.Any()) return true; 
            else return false;
        }

        protected override void ReSequenceBeforeCreate(mstr_process_levelInput input)
        {
            if (input.reSequence != null && input.reSequence == true)
            {
                var restEntries = service.Where(o => o.LevelSequence >= input.LevelSequence && o.ClientID == ((PAIdentity)User.Identity).clientID).OrderBy(p => p.LevelSequence);
                if (restEntries.Any())
                {
                    short sequence = (short)(input.LevelSequence + 1);
                    foreach (var r in restEntries)
                    {
                        if (r.ID != input.ID)
                        {
                            r.LevelSequence = sequence;
                            sequence = (short)(sequence + 1);
                        }
                    }
                }
                service.Save();
            }
        }

        protected override void ReSequenceBeforeEdit(mstr_process_levelInput input)
        {
            if (input.reSequence != null && input.reSequence == true)
            {
                var restEntries = service.Where(o => o.LevelSequence >= input.LevelSequence && o.ClientID == ((PAIdentity)User.Identity).clientID).OrderBy(p => p.LevelSequence);
                if (restEntries.Any())
                {
                    short sequence = (short)(input.LevelSequence + 1);
                    foreach (var r in restEntries)
                    {
                        if (r.ID != input.ID)
                        {
                            r.LevelSequence = sequence;
                            sequence = (short)(sequence + 1);
                        }
                    }
                }
                service.Save();
            }
        }

        protected override void InitiazeSequence(mstr_process_levelInput input)
        {
            var seq = service.Where(o => o.ClientID == ((PAIdentity)User.Identity).clientID);
            if (seq.Any()) { input.LevelSequence = seq.Max(o => o.LevelSequence); input.LevelSequence++; } else { input.LevelSequence = 1; }
        }

    }
}
