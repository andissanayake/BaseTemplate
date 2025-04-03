export enum PriorityLevel {
  None = 0,
  Low = 1,
  Medium = 2,
  High = 3,
}

export interface TodoItem {
  id: number;
  listId: number;
  title?: string;
  note?: string;
  reminder?: Date;
  priority: PriorityLevel;
  done: boolean;
}
