using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Core.Service;
using ProcessAccelerator.WebUI.Dto;
using ProcessAccelerator.WebUI.Mappers;

namespace ProcessAccelerator.WebUI.Controllers
{
    /// <summary>
    /// generic crud controller for entities where there is no difference between the edit and create view
    /// </summary>
    /// <typeparam name="TEntity">the entity</typeparam>
    /// <typeparam name="TInput"> viewmodel </typeparam>
    public abstract class Cruder<TEntity, TInput> : Crudere<TEntity, TInput, TInput>
        where TInput : Input, new()
        where TEntity : Entity, new()
    {
        public Cruder(ICrudService<TEntity> service, IMapper<TEntity, TInput> v, IWorkflowService wf, string FunctionID)
            : base(service, v, v, wf, FunctionID)
        {
        }

        protected override string EditView
        {
            get { return "create"; }
        }
    }
}