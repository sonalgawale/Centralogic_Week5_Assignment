using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using Week5_Sonal_Gawale.DTO;
using Week5_Sonal_Gawale.Entity;

namespace Week5_Sonal_Gawale.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        public string URI = "https://localhost:8081";
        public string PrimaryKey = " C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public string DatabaseName = "TaskManager";
        public string ContainerName = "NewContainer";

        public Container container;

        public TaskController()
        {
            container = GetContainer();
        }

        [HttpPost]
        public async Task<IActionResult> Addtasks(TaskModel taskmodel)
        {
            //Convert TaskModel to Task Entity
            TaskEntity task = new TaskEntity();
            task.TaskId = taskmodel.TaskId;
            task.TaskTitle= taskmodel.TaskTitle;
            task.TaskDescription = taskmodel.TaskDescription;

            //step 2 Assign Mandatory Fields
            task.Id = Guid.NewGuid().ToString();    
            task.UId = task.Id;
            task.DocumentType = "Task";
            task.CreatedBy = "Sonal"; //UID of who created this data
            task.CreatedByName = " ";
            task.CreatedOn = DateTime.Now;
            task.UpdatedBy = " "; //UID
            task.UpdatedByName = " ";  
            task.UpdatedOn = DateTime.Now;
            task.Version = 1;
            task.Active = true;
            task.Archieved = false;

            //step 3 add data to database 
            task = await container.CreateItemAsync(task);

            //step 4 return model to UI
            TaskModel model = new TaskModel();
            model.UId = task.UId;
            model.TaskId = task.TaskId;
            model.TaskTitle = task.TaskTitle;
            model.TaskDescription = task.TaskDescription;  

            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetTaskByUId(string taskUId)
        {
            //step 1 Get All Students
            var task = container.GetItemLinqQueryable<TaskEntity>(true).
                Where(q => q.UId == taskUId && q.DocumentType == "Task" && q.Archieved == false &&
                q.Active == true).AsEnumerable().FirstOrDefault();

            //step 2 Map entity class to model
            TaskModel model = new TaskModel();
            model.UId = task.UId;
            model.TaskId = task.TaskId;
            model.TaskTitle = task.TaskTitle;
            model.TaskDescription = task.TaskDescription;
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetAllStasks()
        {
            //step 1 Get All Students
            var tasks = container.GetItemLinqQueryable<TaskEntity>(true).
                Where(q => q.DocumentType  =="Task" && q.Archieved ==false && 
                q.Active ==true ).AsEnumerable().ToList();

            //step 2 Map all student data
            List<TaskModel> tasksmodelList = new List<TaskModel>();
            foreach(var task in tasks)
            {
                TaskModel model = new TaskModel();
                model.UId = task.UId;
                model.TaskId = task.TaskId;
                model.TaskTitle = task.TaskTitle;
                model.TaskDescription = task.TaskDescription;

                tasksmodelList.Add(model);
            }
            return Ok(tasksmodelList);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(TaskModel taskmodel)
        {
            //Get Existing Data
            var existingTask = container.GetItemLinqQueryable<TaskEntity>(true).
                Where(q => q.UId == taskmodel.UId && q.DocumentType == "Task" && q.Archieved == false && q.Active == true).AsEnumerable().FirstOrDefault();
            existingTask.Archieved = true;
            await container.ReplaceItemAsync(existingTask, existingTask.Id);
            //2 . Assign Mandatory fields
            existingTask.Id = Guid.NewGuid().ToString();
            existingTask.UpdatedBy = " ";
            existingTask.UpdatedByName = " ";
            existingTask.UpdatedOn = DateTime.Now;
            existingTask.Version = existingTask.Version+1;
            existingTask.Active = true;
            existingTask.Archieved = false;

            //3. assign UI Model Fields 
            existingTask.UId  = taskmodel.UId;
            existingTask.TaskId = taskmodel.TaskId;
            existingTask.TaskTitle = taskmodel.TaskTitle;   
            existingTask.TaskDescription = taskmodel.TaskDescription;

            //4. add data to db
            existingTask = await container.CreateItemAsync(existingTask);

            //5 return model

            TaskModel model = new TaskModel();
            model.UId = existingTask.UId;
            model.TaskId = existingTask.TaskId;
            model.TaskTitle = existingTask.TaskTitle;
            model.TaskDescription = existingTask.TaskDescription;

            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(string taskUId)
        {
            //1.Get Existing data
            var existingtask = container.GetItemLinqQueryable<TaskEntity>(true).
               Where(q => q.UId == taskUId && q.DocumentType == "Task" && q.Archieved == false &&
               q.Active == true).AsEnumerable().FirstOrDefault();
            existingtask.Active= false;
            await container.ReplaceItemAsync(existingtask, existingtask.Id);
            return Ok(true);
        }

        private Container GetContainer()
        {
            CosmosClient cosmosClient = new CosmosClient(URI, PrimaryKey);
            Database db = cosmosClient.GetDatabase(DatabaseName);
            Container container = db.GetContainer(ContainerName);
            return container;
        }
    }
}
