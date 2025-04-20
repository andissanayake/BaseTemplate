import { create } from "zustand";
import { TodoItemService } from "./todoItemService";
import { TodoItem } from "./TodoItemModel";
import { handleResult } from "../../common/handleResult";

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
    set({ loading: true });
    const { currentPage, pageSize } = useTodoItemStore.getState();
    const response = await TodoItemService.fetchTodoItems(
      listId,
      currentPage,
      pageSize
    );
    handleResult(response, {
      onSuccess: (data) => {
        set({
          todoItemList: data?.items || [],
          totalCount: data?.totalCount || 0,
          loading: false,
        });
      },
      onFinally: () => {
        set({ loading: false });
      },
    });
  },

  updateItemStatus: async (id, done, listId) => {
    set({ loading: true });
    const response = await TodoItemService.updateTodoItemStatus({ id, done });
    handleResult(response, {
      onSuccess: () => {
        useTodoItemStore.getState().fetchTodoItems(listId);
      },
      onFinally: () => {
        set({ loading: false });
      },
    });
  },

  deleteTodoItem: async (id, listId) => {
    set({ loading: true });
    const { currentPage, pageSize, totalCount } = useTodoItemStore.getState();
    const response = await TodoItemService.deleteTodoItem(id);
    handleResult(response, {
      onSuccess: () => {
        const newTotalCount = totalCount - 1;
        const lastPage = Math.ceil(newTotalCount / pageSize);
        if (currentPage > lastPage && lastPage > 0) {
          set({ currentPage: lastPage });
        }
        useTodoItemStore.getState().fetchTodoItems(listId);
      },
      onFinally: () => {
        set({ loading: false });
      },
    });
  },

  setPagination: (page, size) => set({ currentPage: page, pageSize: size }),

  createTodoItem: async (data, listId) => {
    set({ loading: true });
    const response = await TodoItemService.createTodoItem(data);
    handleResult(response, {
      onSuccess: () => {
        useTodoItemStore.getState().fetchTodoItems(listId);
      },
      onFinally: () => {
        set({ loading: false });
      },
    });
  },

  updateTodoItem: async (data, listId) => {
    set({ loading: true });
    const response = await TodoItemService.updateTodoItem(data);
    handleResult(response, {
      onSuccess: () => {
        useTodoItemStore.getState().fetchTodoItems(listId);
      },
      onFinally: () => {
        set({ loading: false });
      },
    });
  },
  setTodoItemEdit: (data) => set({ editTodoItem: data }),
}));
