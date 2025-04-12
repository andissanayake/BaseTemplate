import React, { useEffect } from "react";
import { Modal, Form, Input, DatePicker, Select, notification } from "antd";
import { useTodoItemStore } from "./todoItemStore";
import { TodoItemService } from "./todoItemService";
import dayjs from "dayjs";
import { useParams } from "react-router-dom";
import { TodoItem } from "./TodoItemModel";

interface TodoItemEditProps {
  visible: boolean;
  onClose: () => void;
  todoItem: TodoItem;
}

const TodoItemEdit: React.FC<TodoItemEditProps> = ({
  visible,
  onClose,
  todoItem,
}) => {
  const [form] = Form.useForm();
  const { updateTodoItem } = useTodoItemStore();
  const { listId } = useParams();

  useEffect(() => {
    if (todoItem) {
      form.setFieldsValue({
        ...todoItem,
        reminder: todoItem.reminder ? dayjs(todoItem.reminder) : undefined,
      });
    }
  }, [todoItem, form]);

  const handleSave = async () => {
    form.validateFields().then(async (values) => {
      const payload: TodoItem = {
        ...values,
        reminder: values.reminder ? dayjs(values.reminder).format() : null,
        id: todoItem.id,
        listId: +listId!,
      };

      try {
        await updateTodoItem(payload, +listId!);
        notification.success({ message: "Todo item updated successfully!" });
        onClose();
      } catch (error) {
        console.error("Error updating todo item:", error);
        notification.error({ message: "Failed to update todo item!" });
      }
    });
  };

  return (
    <Modal
      title="Edit Todo Item"
      open={visible}
      onCancel={onClose}
      onOk={handleSave}
    >
      <Form form={form} layout="vertical">
        <Form.Item label="Title" name="title" rules={[{ required: true }]}>
          <Input />
        </Form.Item>
        <Form.Item label="Note" name="note">
          <Input.TextArea rows={3} />
        </Form.Item>
        <Form.Item label="Reminder" name="reminder">
          <DatePicker
            showTime
            format="YYYY-MM-DD HH:mm"
            style={{ width: "100%" }}
          />
        </Form.Item>
        <Form.Item label="Priority" name="priority">
          <Select options={TodoItemService.getPriorityLevels()} />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default TodoItemEdit;
