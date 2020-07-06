using AutoMapper;
using FluentMessenger.API.Dtos;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FluentMessenger.API.Extensions {
    public static class JsonPatchDocumentExtention {
        public static void ApplyToCustom<T, TEntity>(this JsonPatchDocument<T> patchDoc, TEntity objectToApplyTo,
            ModelStateDictionary modelState, IMapper _mapper) where T : class {
            if (modelState.IsValid) {
                var group = objectToApplyTo as Group;
                foreach (var operation in patchDoc.Operations) {
                    var partsOfPath = operation.path.Split('/');
                    switch (operation.OperationType) {
                        case OperationType.Add:
                            if (partsOfPath[1].ToLower() == "contacts") {
                                if (partsOfPath.Length > 2) {
                                    var contacts = JsonConvert.DeserializeObject<IEnumerable<ContactDto>>(
                                        operation.value.ToString());
                                    group.Contacts.AddRange(_mapper.Map<IEnumerable<Contact>>(contacts));
                                    break;
                                }
                                var contact = JsonConvert.DeserializeObject<ContactDto>(operation.value.ToString());
                                group.Contacts.Add(_mapper.Map<Contact>(contact));
                            }
                            else {
                                var message = JsonConvert.DeserializeObject<MessageDto>(operation.value.ToString());
                                group.Messages.Add(new Message {
                                    Value = message.Message,
                                    SentTime = message.SentTime,
                                    GroupId = group.Id,
                                });
                            }

                            break;
                        case OperationType.Remove:
                            if (partsOfPath.Length == 3) {
                                var id = int.Parse(partsOfPath[2]);
                                if (partsOfPath[1].ToLower() == "contacts") {
                                    var item = group.Contacts.FirstOrDefault(x => x.Id == id);
                                    if (item == null) {
                                        throw new PatchIdNotFoundException(nameof(id));
                                    }
                                    group.Contacts.Remove(item);
                                }
                                else {
                                    var message = group.Messages.FirstOrDefault(x => x.Id == id);
                                    if (message == null) {
                                        throw new PatchIdNotFoundException(nameof(id));
                                    }
                                    group.Messages.Remove(message);
                                }
                            }
                            break;
                        case OperationType.Replace:
                            if (partsOfPath[1].ToLower() == "contacts") {
                                var body = JsonConvert.DeserializeObject<ContactDto>(operation.value.ToString());
                                var itemToChange = group.Contacts.FirstOrDefault(x => x.Id == body.Id);
                                if (itemToChange == null) {
                                    throw new PatchIdNotFoundException(nameof(body.Id));
                                }
                                itemToChange.FullName = body.FullName;
                                itemToChange.PhoneNumber = body.Number;
                            }
                            else if (partsOfPath[1].ToLower() == "messages") {
                                var body = JsonConvert.DeserializeObject<MessageDto>(operation.value.ToString());
                                var itemToChange = group.Messages.FirstOrDefault(x => x.Id == body.Id);
                                if (itemToChange == null) {
                                    throw new PatchIdNotFoundException(nameof(body.Id));
                                }
                                itemToChange.Value = body.Message;
                                itemToChange.SentTime = body.SentTime;
                            }
                            else {
                                if (operation.value == null) {
                                    throw new PatchIdNotFoundException(nameof(operation.value));
                                }
                                group.Title = operation.value.ToString();
                            }

                            break;
                    }
                }
            }
        }
    }
}
