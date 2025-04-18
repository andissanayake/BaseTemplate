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
  fetchTodoItems: (listId: number) => Promise<void>;
  updateItemStatus: (
    id: number,
    done: boolean,
    listId: number
  ) => Promise<void>;
  deleteTodoItem: (id: number, listId: number) => Promise<void>;
  setPagination: (page: number, size: number) => void;
  createTodoItem: (data: TodoItem, listId: number) => Promise<void>;
  updateTodoItem: (data: TodoItem, listId: number) => Promise<void>;
}

export const useTodoItemStore = create<TodoItemState>((set) => ({
  todoItemList: [],
  totalCount: 0,
  loading: false,
  currentPage: 1,
  pageSize: 5,
  editTodoItem: null,

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

  updateItemStatus: async (id, done, listId) => {
    set({ loading: true });
    try {
      await TodoItemService.updateTodoItemStatus({ id, done });
      set({ loading: false });
      useTodoItemStore.getState().fetchTodoItems(listId);
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },

  deleteTodoItem: async (id, listId) => {
    set({ loading: true });
    try {
      const { currentPage, pageSize, totalCount } = useTodoItemStore.getState();
      // Deleting the item
      await TodoItemService.deleteTodoItem(id);
      // Check if the current page becomes empty after deletion
      const newTotalCount = totalCount - 1;
      const lastPage = Math.ceil(newTotalCount / pageSize);
      // If we are on the last page and the number of items is less than pageSize, move to the previous page
      if (currentPage > lastPage && lastPage > 0) {
        set({ currentPage: lastPage });
      }
      // Fetch the updated todo items
      await useTodoItemStore.getState().fetchTodoItems(listId);
      set({ loading: false });
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },

  setPagination: (page, size) => set({ currentPage: page, pageSize: size }),

  createTodoItem: async (data, listId) => {
    set({ loading: true });
    try {
      await TodoItemService.createTodoItem(data);
      set({ loading: false });
      useTodoItemStore.getState().fetchTodoItems(listId);
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },

  updateTodoItem: async (data, listId) => {
    set({ loading: true });
    try {
      await TodoItemService.updateTodoItem(data);
      set({ loading: false });
      useTodoItemStore.getState().fetchTodoItems(listId);
    } catch (error) {
      console.error(error);
      set({ loading: false });
    }
  },
  setTodoItemEdit: (data) => set({ editTodoItem: data }),
}));
