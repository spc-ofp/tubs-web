// -----------------------------------------------------------------------
// <copyright file="AbstractBindAttribute.cs" company="Secretariat of the Pacific Community">
// Copyright (C) 2011 Secretariat of the Pacific Community
// </copyright>
// -----------------------------------------------------------------------

namespace TubsWeb.Core
{
    /*
     * This file is part of TUBS.
     *
     * TUBS is free software: you can redistribute it and/or modify
     * it under the terms of the GNU Affero General Public License as published by
     * the Free Software Foundation, either version 3 of the License, or
     * (at your option) any later version.
     *  
     * TUBS is distributed in the hope that it will be useful,
     * but WITHOUT ANY WARRANTY; without even the implied warranty of
     * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
     * GNU Affero General Public License for more details.
     *  
     * You should have received a copy of the GNU Affero General Public License
     * along with TUBS.  If not, see <http://www.gnu.org/licenses/>.
     */
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// MVC can't handle an abstract class as a controller parameter.
    /// This is not entirely surprising since the class is abstract.
    /// This class allows the default model binder to handle the situation via a
    /// hidden parameter.
    /// The implementation is directly copied from Martin Booth's solution to this
    /// Stack Overflow question:
    /// http://stackoverflow.com/questions/5460081/asp-net-mvc-3-defaultmodelbinder-with-inhertance-polymorphism
    /// </summary>
    public class AbstractBindAttribute : CustomModelBinderAttribute
    {
        public string ConcreteTypeParameter { get; set; }

        public override IModelBinder GetBinder()
        {
            return new AbstractModelBinder(ConcreteTypeParameter);
        }

        private class AbstractModelBinder : DefaultModelBinder
        {
            private readonly string concreteTypeParameterName;

            public AbstractModelBinder(string concreteTypeParameterName)
            {
                this.concreteTypeParameterName = concreteTypeParameterName;
            }

            protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
            {
                var concreteTypeValue = bindingContext.ValueProvider.GetValue(concreteTypeParameterName);

                if (concreteTypeValue == null)
                    throw new Exception("Concrete type value not specified for abstract class binding");

                // The default implementation of this attribute can only deal with types in the executing assembly.
                // This doesn't work when the entity is defined in a different assembly.
                Type concreteType = null;
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in loadedAssemblies)
                {
                    concreteType = assembly.GetType(concreteTypeValue.AttemptedValue);
                    if (null != concreteType)
                    {
                        break;
                    }
                }

                if (concreteType == null)
                    throw new Exception("Cannot create abstract model");

                if (!concreteType.IsSubclassOf(modelType))
                    throw new Exception("Incorrect model type specified");

                var concreteInstance = Activator.CreateInstance(concreteType);

                bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => concreteInstance, concreteType);

                return concreteInstance;
            }
        }
    }
}