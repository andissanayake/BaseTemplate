/* eslint-disable @typescript-eslint/no-explicit-any */
import React from "react";
import { Modal, Form, Input, notification } from "antd";
import { TodoItemService } from "./todoItemService";
import { useTodoItemStore } from "./todoItemStore";

interface TodoItemModalProps {
  visible: boolean;
  onClose: () => void;
}

const TodoItemCreate: React.FC<TodoItemModalProps> = ({ visible, onClose }) => {
  const [form] = Form.useForm();
  const { fetchTodoItems } = useTodoItemStore();

  const handleSaveTodoItem = () => {
    form.validateFields().then(async (values) => {
      try {
        await TodoItemService.createTodoItem(values);
        notification.success({ message: "Operation successful!" });
        onClose();
        form.resetFields();
        await fetchTodoItems();
      } catch (error: any) {
        console.error("Error creating todo item:", error);
        notification.error({ message: "Failed to create todo item!" });
      }
    });
  };

  return (
    <Modal
      title={"Add New Todo List"}
      open={visible}
      onCancel={onClose}
      onOk={handleSaveTodoItem}
    >
      <Form form={form} layout="vertical">
        <Form.Item
          label="Todo List Title"
          name="title"
          rules={[
            { required: true, message: "Please enter the todo list title!" },
          ]}
        >
          <Input placeholder="Enter todo list title" />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default TodoItemCreate;
