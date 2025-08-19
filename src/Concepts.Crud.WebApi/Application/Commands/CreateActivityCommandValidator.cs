using FluentValidation;

namespace Concepts.Crud.WebApi.Application.Commands;

public class CreateActivityCommandValidator
: AbstractValidator<CreateActivityCommand>
{
   public CreateActivityCommandValidator(ILogger<CreateActivityCommandValidator> logger)
   {
      //TODO: fillme: RuleForEach(x=>x.Data)....
      
      if (logger.IsEnabled(LogLevel.Trace))
      {
         logger.LogTrace("INSTANCE CREATED - {ClassName}", GetType().Name);
      }
   } 
}