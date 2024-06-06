using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace StudentProject.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(propertyName);
            if (value == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            else
            {
                try
                {
                    var deSerializationValue = JsonConvert.DeserializeObject<T>(value.FirstValue);
                    bindingContext.Result = ModelBindingResult.Success(deSerializationValue);

                }
                catch (Exception ex)
                {
                    bindingContext.ModelState.TryAddModelError(propertyName, "The provided value and type are not matching");
                }
                return Task.CompletedTask;
            }
        }
    }
}
