package main

import (
	"encoding/json"
	"log"
	"net/http"
	"time"

	"github.com/google/uuid"
	"github.com/gorilla/mux"
)

type Todo struct {
	Id        uuid.UUID `json:"id"`
	Name      string    `json:"name"`
	Status    bool      `json:"status"`
	CreatedAt time.Time `json:"createdAt"`
}

type TodoInputDto struct {
	Name   string `json:"name"`
	Status bool   `json:"status"`
}

var todos []Todo

func main() {
	// Add three default todos
	todos = append(todos, Todo{
		Id:        uuid.New(),
		Name:      "Learn Go",
		Status:    false,
		CreatedAt: time.Now().UTC(),
	})
	todos = append(todos, Todo{
		Id:        uuid.New(),
		Name:      "Build a REST API",
		Status:    false,
		CreatedAt: time.Now().UTC(),
	})
	todos = append(todos, Todo{
		Id:        uuid.New(),
		Name:      "Deploy the application",
		Status:    false,
		CreatedAt: time.Now().UTC(),
	})

	router := mux.NewRouter()

	router.HandleFunc("/todos", createTodo).Methods("POST")
	router.HandleFunc("/todos", getTodos).Methods("GET")
	router.HandleFunc("/todos/{id}", getTodoById).Methods("GET")
	router.HandleFunc("/todos/{id}", updateTodo).Methods("PUT")
	router.HandleFunc("/todos/{id}", deleteTodo).Methods("DELETE")

	log.Fatal(http.ListenAndServe(":8080", router))
}

func createTodo(w http.ResponseWriter, r *http.Request) {
	var todoInput TodoInputDto
	err := json.NewDecoder(r.Body).Decode(&todoInput)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	todo := Todo{
		Id:        uuid.New(),
		Name:      todoInput.Name,
		Status:    todoInput.Status,
		CreatedAt: time.Now().UTC(),
	}

	todos = append(todos, todo)
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusCreated)
	json.NewEncoder(w).Encode(todo)
}

func getTodos(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(todos)
}

func getTodoById(w http.ResponseWriter, r *http.Request) {
	params := mux.Vars(r)
	id, err := uuid.Parse(params["id"])
	if err != nil {
		http.Error(w, "Invalid ID format", http.StatusBadRequest)
		return
	}

	for _, todo := range todos {
		if todo.Id == id {
			w.Header().Set("Content-Type", "application/json")
			json.NewEncoder(w).Encode(todo)
			return
		}
	}

	http.Error(w, "Todo not found", http.StatusNotFound)
}

func updateTodo(w http.ResponseWriter, r *http.Request) {
	params := mux.Vars(r)
	id, err := uuid.Parse(params["id"])
	if err != nil {
		http.Error(w, "Invalid ID format", http.StatusBadRequest)
		return
	}

	var updatedTodoDto TodoInputDto
	err = json.NewDecoder(r.Body).Decode(&updatedTodoDto)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	for index, todo := range todos {
		if todo.Id == id {
			todos[index].Name = updatedTodoDto.Name
			todos[index].Status = updatedTodoDto.Status
			w.Header().Set("Content-Type", "application/json")
			json.NewEncoder(w).Encode(todos[index])
			return
		}
	}

	http.Error(w, "Todo not found", http.StatusNotFound)
}

func deleteTodo(w http.ResponseWriter, r *http.Request) {
	params := mux.Vars(r)
	id, err := uuid.Parse(params["id"])
	if err != nil {
		http.Error(w, "Invalid ID format", http.StatusBadRequest)
		return
	}

	for index, todo := range todos {
		if todo.Id == id {
			todos = append(todos[:index], todos[index+1:]...)
			w.WriteHeader(http.StatusOK)
			return
		}
	}

	http.Error(w, "Todo not found", http.StatusNotFound)
}
