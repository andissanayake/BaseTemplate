import { create } from "zustand";
import { TodoItemService } from "./todoItemService";
import { TodoItem } from "./TodoItemModel";

interface TodoItemState {
  todoItemList: TodoItem[];
  totalCount: number;
  loading: boolean;
  currentPage: number;
  pageSize: number;
  editTodoItem: TodoItem | null;
  setTodoItemEdit: (data: TodoItem | null) => void;
  fetchTodoItems: (listId: number) => void; // Simplified to use store's pagination
  updateItemStatus: (id: number, done: boolean, listId: number) => void;
  deleteTodoItem: (id: number, listId: number) => void;
  setPagination: (page: number, size: number) => void;
  createTodoItem: (data: TodoItem, listId: number) => void;
  updateTodoItem: (data: TodoItem, listId: number) => void;
}

export const useTodoItemStore = create<TodoItemState>((set) => ({
  todoItemList: [],
  totalCount: 0,
  loading: false,
  currentPage: 1,
  pageSize: 5,
  editTodoItem: null,

  // Fetch todo items with current pagination
  fetchTodoItems: async (listId) => {
    const { currentPage, pageSize } = useTodoItemStore.getState();
    set({ loading: true });
    try {
      const response = await TodoItemService.fetchTodoItems(
        listId,
        currentPage,
        pageSize
      );
      set({
        todoItemList: response.data.items,
        totalCount: response.data.totalCount,
        loading: false,
      });
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },

  // Update the 'done' status of a todo item
  updateItemStatus: async (id, done, listId) => {
    set({ loading: true });
    try {
      await TodoItemService.updateTodoItemStatus({ id, done });
      set({ loading: false });
      // Refetch todo items after status update
      useTodoItemStore.getState().fetchTodoItems(listId);
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },

  // Delete a todo item
  deleteTodoItem: async (id, listId) => {
    set({ loading: true });
    try {
      await TodoItemService.deleteTodoItem({ id } as TodoItem);
      set({ loading: false });
      // Refetch todo items after deletion
      await useTodoItemStore.getState().fetchTodoItems(listId);
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },

  // Set pagination details
  setPagination: (page, size) => set({ currentPage: page, pageSize: size }),

  // Create a new todo item
  createTodoItem: async (data, listId) => {
    set({ loading: true });
    try {
      await TodoItemService.createTodoItem(data);
      set({ loading: false });
      // Refetch todo items after creation
      useTodoItemStore.getState().fetchTodoItems(listId);
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },

  // Update an existing todo item
  updateTodoItem: async (data, listId) => {
    set({ loading: true });
    try {
      await TodoItemService.updateTodoItem(data);
      set({ loading: false });
      // Refetch todo items after update
      useTodoItemStore.getState().fetchTodoItems(listId);
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },

  // Set the todo item to be edited
  setTodoItemEdit: (data) => set({ editTodoItem: data }),
}));
