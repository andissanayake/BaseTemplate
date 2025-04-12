import React from "react";
import { Modal, Form, Input, DatePicker, Select, notification } from "antd";
import { useTodoItemStore } from "./todoItemStore";
import { TodoItemService } from "./todoItemService";
import dayjs from "dayjs";
import { useParams } from "react-router-dom";
import { TodoItem } from "./TodoItemModel";

interface TodoItemCreateProps {
  visible: boolean;
  onClose: () => void;
}

const TodoItemCreate: React.FC<TodoItemCreateProps> = ({
  visible,
  onClose,
}) => {
  const [form] = Form.useForm();
  const { createTodoItem } = useTodoItemStore();
  const { listId } = useParams();

  const handleSave = async () => {
    form.validateFields().then(async (values) => {
      const payload: TodoItem = {
        ...values,
        reminder: values.reminder ? dayjs(values.reminder).format() : null,
        listId: +listId!,
      };

      try {
        await createTodoItem(payload, +listId!);
        notification.success({ message: "Todo item created successfully!" });
        form.resetFields();
        onClose();
      } catch (error) {
        console.error("Error creating todo item:", error);
        notification.error({ message: "Failed to create todo item!" });
      }
    });
  };

  return (
    <Modal
      title="Create Todo Item"
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

export default TodoItemCreate;
