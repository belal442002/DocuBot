using AzureOpenAI_Task2.Models.ViewModels;
using AzureOpenAI_Task2.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using AzureOpenAI_Task2.Services;
using AzureOpenAI_Task2.UOW;
using AzureOpenAI_Task2.MappingProfile;
using AzureOpenAI_Task2.Helper;
using Microsoft.EntityFrameworkCore;

namespace AzureOpenAI_Task2.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentService _documentService;
        private readonly IFileService _fileService;
        private readonly IOpenAIService _openAIService;

        public DocumentController(IUnitOfWork unitOfWork,
                                  IDocumentService documentService,
                                  IFileService fileService,
                                  IOpenAIService openAIService)
        {
            _unitOfWork = unitOfWork;
            _documentService = documentService;
            _fileService = fileService;
            _openAIService = openAIService;
        }


        [HttpPost]
        [Route("upload-document")]
        public async Task<IActionResult> UploadDocument(IFormFile documentFile)
        {
            if(documentFile == null || documentFile.Length == 0)
            {
                return BadRequest("No file was uploaded");
            }
            
            try
            {
                var extractedText = await _documentService.ExtractTextAsync(documentFile);
                var chatSession = new Chat()
                {
                    DocumentContent = extractedText,
                    DocumentPath = await _fileService.SaveFileAsync(documentFile),
                    Messages = new List<ChatMessage>()
                };

                await _unitOfWork.ChatRepository.AddAsync(chatSession);
                await _unitOfWork.SaveChangesAsync();

                
                return View("ChatSession", chatSession);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            } 
        }

        [HttpGet]
        [Route("GetOldSession")]
        public async Task<IActionResult> GetOldSession(int sessionId)
        {
            var oldSession = await _unitOfWork.ChatRepository.FirstOrDefaultAsync(
                filter: c => c.SessionId == sessionId,
                include: c => c.Include(c => c.Messages)
                );

            return View("ChatSession", oldSession);
        }

        [HttpGet]
        [Route("DeleteSession")]
        public async Task<IActionResult> DeleteSession(int sessionId)
        {
            var session = await _unitOfWork.ChatRepository.GetByIdAsync(sessionId);
            if (session is null)
            {
                return NotFound();
            }

            _unitOfWork.ChatRepository.Delete(session);
            await _unitOfWork.SaveChangesAsync();

            var oldSessions = await _unitOfWork.ChatRepository.GetAllAsync();
            return View("OldSessions", oldSessions);
        }

        [HttpPost]
        [Route("/SendMessage")]
        public async Task<IActionResult> SendMessage(string text, Chat chat)
        {
            //var doc = "As an enthusiastic ASP.NET Core," +
            //    " I am deeply passionate about building robust web applications " +
            //    "\r\nusing this versatile framework." +
            //    " My career objective is to secure a position" +
            //    " that allows me to expand \r\nmy expertise in web development" +
            //    " and contribute to the" +
            //    " success of a forward-thinking organization.";

            List<ChatMessage> newChatMessages = new List<ChatMessage>();
            List<ChatMessage> MessagesToRecall = new List<ChatMessage>();


            if (chat.Messages == null || chat.Messages.Count == 0)
            {
                MessagesToRecall.Add(new ChatMessage
                {
                    Role = Roles.system.ToString(),
                    SessionId = chat.SessionId,
                    Content = "You are a helpful assistant that answers questions about uploaded documents."
                });
                await Task.Delay(1000);

                MessagesToRecall.Add(new ChatMessage
                {
                    Role = Roles.user.ToString(),
                    SessionId = chat.SessionId,
                    Content = $"Here is the Document: {chat.DocumentContent}."
                });
                
                newChatMessages.AddRange(MessagesToRecall);
            }
            else
            {
                MessagesToRecall = await _unitOfWork.ChatMessageRepository.GetAsync(
                    filter: m => m.SessionId == chat.SessionId,
                    orderBy: m => m.OrderBy(m => m.TimeStamp)
                    );
            }

            await Task.Delay(1000);

            var userQuestion = new ChatMessage
            {
                Role = Roles.user.ToString(),
                SessionId = chat.SessionId,
                Content = text
            };

            MessagesToRecall.Add(userQuestion);
            newChatMessages.Add(userQuestion);
            try
            {
                var modelResponseMessage = await _openAIService.
                GetChatCompletionUsingSDKAsync(MessagesToRecall.ToMessageList());
                
                newChatMessages.Add(modelResponseMessage.ToMessageModel(chat.SessionId));
                await _unitOfWork.ChatMessageRepository.AddRangeAsync(newChatMessages);
                await _unitOfWork.SaveChangesAsync();
                
                var fullChat = await _unitOfWork.ChatRepository.FirstOrDefaultAsync(
                    filter: c => c.SessionId == chat.SessionId,
                    include: c => c.Include(c => c.Messages.OrderBy(m => m.TimeStamp))
                    );

                return View("ChatSession", fullChat);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
