﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Metadata;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Data.Entity
{
    /// <summary>
    /// Entity base object.
    /// </summary>
    public abstract class EntityBase : NotifyBase, IEntity
    {
        private EntityMetadata _Metadata;
        /// <summary>
        /// Get the metadata of entity.
        /// </summary>
        protected EntityMetadata Metadata
        {
            get
            {
                if (_Metadata == null)
                    _Metadata = EntityAnalyzer.GetMetadata(GetType());
                return _Metadata;
            }
        }
        
        /// <summary>
        /// Get or set the id of entity.
        /// </summary>
        [Key]
        [Required]
        [Hide]
        public virtual Guid Index { get { return (Guid)GetValue(); } set { SetValue(value); } }

        /// <summary>
        /// Get or set the create date of entity.
        /// </summary>
        [Required]
        [Hide]
        [Column(TypeName = "Datetime2")]
        public virtual DateTime CreateDate { get { return (DateTime)GetValue(); } set { SetValue(value); } }

        /// <summary>
        /// Call when entity created.
        /// </summary>
        public virtual void OnCreateCompleted()
        {

        }

        /// <summary>
        /// Call when entity edited.
        /// </summary>
        public virtual void OnEditCompleted()
        {

        }

        /// <summary>
        /// Get is the entity can remove.
        /// </summary>
        /// <returns></returns>
        [Hide]
        [NotMapped]
        public virtual bool IsRemoveAllowed
        {
            get { return true; }
        }

        /// <summary>
        /// Get is the entity can edit.
        /// </summary>
        /// <returns></returns>
        [Hide]
        [NotMapped]
        public virtual bool IsEditAllowed
        {
            get { return true; }
        }

        /// <summary>
        /// Return a string that entity display.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Metadata.DisplayProperty == null)
                return base.ToString();
            return Metadata.DisplayProperty.Property.GetValue(this, null).ToString();
        }

        //private ReadOnlyCollection<ValidationResult> _NoError = new ReadOnlyCollection<ValidationResult>(new List<ValidationResult>());
        /// <summary>
        /// Ensure that entity is valid.
        /// </summary>
        /// <param name="validationContext">Validation context.</param>
        /// <returns>Collection that include error messages.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();
            foreach (var property in Metadata.Properties)
            {
                validationContext.MemberName = property.Property.Name;
                validationContext.DisplayName = property.Name;
                var list = property.Property.GetCustomAttributes<ValidationAttribute>(true);
                foreach (var item in list)
                {
                    var r = item.GetValidationResult(property.Property.GetValue(this), validationContext);
                    if (r != null && r != ValidationResult.Success)
                        result.Add(r);
                }
            }
            return result;
        }
    }
}
