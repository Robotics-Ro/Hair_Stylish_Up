using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Prism_HairStylishUp
{
    public abstract class AddBindableBase : BindableBase, INotifyDataErrorInfo
    {
        public Prism.Mvvm.ErrorsContainer<string> errorsContainer { get; set; }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public AddBindableBase()
        {
            this.errorsContainer = new ErrorsContainer<string>(OnErrorsChanged);
        }

        /// <summary>
        /// ValidateProperty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            var context = new ValidationContext(this) { MemberName = propertyName };
            var validationErrors = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (!Validator.TryValidateProperty(value, context, validationErrors))
            {
                var errors = validationErrors.Select(error => error.ErrorMessage);
                this.errorsContainer.SetErrors(propertyName, errors);
            }
            else
            {
                this.errorsContainer.ClearErrors(propertyName);
            }
        }

        /// <summary>
        /// HasErrors
        /// </summary>
        /// <returns>bool</returns>
        public bool HasErrors
        {
            get { return this.errorsContainer.HasErrors; }
        }

        /// <summary>
        /// GetErrors
        /// </summary>
        /// <param name=" propertyName"></param>
        /// <returns>IEnumerable</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return this.errorsContainer.GetErrors(propertyName);
        }

        /// <summary>
        /// OnErrorsChanged
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnErrorsChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.ErrorsChanged;
            if (handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// ErrorsContainer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="OnErrorsChanged"></param>
        internal void ErrorsContainer<T1>(Action<string> OnErrorsChanged)
        {
            throw new NotImplementedException();
        }

    }
}
