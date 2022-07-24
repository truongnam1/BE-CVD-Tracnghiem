using Tracnghiem.Common;
using Tracnghiem.Entities;
using Tracnghiem.Helpers;
using Tracnghiem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueSight.Common;
using TrueSight.PER.Entities;

namespace Tracnghiem.Services.MRole
{
    public interface IFieldValidator : IServiceScoped
    {
        Task<bool> Update(Field Field);
    }

    public class FieldValidator : IFieldValidator
    {
        public enum ErrorTranslatedName
        {
            IdNotExisted,
            TranslatedNameExisted,
            TranslatedNameNotExisted,
            TranslatedNameEmpty,
            TranslatedNameHasSpecialCharacter,
            TranslatedNameOverLength,
            NameEmpty,
            NameOverLength,
            NameExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public FieldValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Field Field)
        {
            FieldFilter FieldFilter = new FieldFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Field.Id },
                Selects = FieldSelect.Id
            };

            int count = await UOW.FieldRepository.Count(FieldFilter);
            if (count == 0)
                Field.AddError(nameof(FieldValidator), nameof(Field.Id), ErrorTranslatedName.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> ValidateTranslatedName(Field Field)
        {
            if (string.IsNullOrWhiteSpace(Field.TranslatedName))
                Field.AddError(nameof(FieldValidator), nameof(Field.TranslatedName), ErrorTranslatedName.TranslatedNameEmpty);
            else
            {
                var TranslatedName = Field.TranslatedName;
                if (Field.TranslatedName.Contains(" ") || !TranslatedName.ChangeToEnglishChar().Equals(Field.TranslatedName))
                {
                    Field.AddError(nameof(FieldValidator), nameof(Field.TranslatedName), ErrorTranslatedName.TranslatedNameHasSpecialCharacter);
                }
                else if (Field.TranslatedName.Length > 255)
                    Field.AddError(nameof(FieldValidator), nameof(Field.TranslatedName), ErrorTranslatedName.TranslatedNameOverLength);
                else
                {
                    FieldFilter FieldFilter = new FieldFilter
                    {
                        Skip = 0,
                        Take = 10,
                        Id = new IdFilter { NotEqual = Field.Id },
                        TranslatedName = new StringFilter { Equal = Field.TranslatedName },
                        MenuId = new IdFilter { Equal = Field.MenuId },
                        FieldTypeId = new IdFilter { Equal = Field.FieldTypeId },
                        Selects = FieldSelect.TranslatedName
                    };
                    int count = await UOW.FieldRepository.Count(FieldFilter);
                    if (count != 0)
                        Field.AddError(nameof(FieldValidator), nameof(Field.TranslatedName), ErrorTranslatedName.TranslatedNameExisted);
                }

            }
            return Field.IsValidated;
        }
        public async Task<bool> ValidateName(Field Field)
        {
            if (string.IsNullOrWhiteSpace(Field.Name))
                Field.AddError(nameof(FieldValidator), nameof(Field.Name), ErrorTranslatedName.NameEmpty);
            else
            {
                if (Field.Name.Length > 255)
                    Field.AddError(nameof(FieldValidator), nameof(Field.Name), ErrorTranslatedName.NameOverLength);
                FieldFilter FieldFilter = new FieldFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Field.Id },
                    MenuId = new IdFilter { Equal = Field.MenuId },
                    FieldTypeId = new IdFilter { Equal = Field.FieldTypeId },
                    Name = new StringFilter { Equal = Field.Name },
                    Selects = FieldSelect.Name
                };

                int count = await UOW.FieldRepository.Count(FieldFilter);
                if (count != 0)
                    Field.AddError(nameof(FieldValidator), nameof(Field.Name), ErrorTranslatedName.NameExisted);
            }
            return Field.IsValidated;

        }

        public async Task<bool> Update(Field Field)
        {
            if (await ValidateId(Field))
            {
                await ValidateTranslatedName(Field);
                await ValidateName(Field);
            }
            return Field.IsValidated;
        }
    }
}
