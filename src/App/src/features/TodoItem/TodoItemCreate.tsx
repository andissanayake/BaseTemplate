/* eslint-disable @typescript-eslint/no-explicit-any */
import React from "react";
import { Modal, Form, Input, notification } from "antd";
import { TodoItemService } from "./todoItemService";
import { useParams } from "react-router-dom";

interface TodoItemModalProps {
  visible: boolean;
  onClose: () => void;
}

const TodoItemCreate: React.FC<TodoItemModalProps> = ({ visible, onClose }) => {
  const [form] = Form.useForm();
  const { listId } = useParams();
  if (!listId) return null;
  const handleSaveTodoItem = () => {
    form.validateFields().then(async (values) => {
      try {
        await TodoItemService.createTodoItem({ ...values, listId: +listId });
        notification.success({ message: "Operation successful!" });
        onClose();
        form.resetFields();
      } catch (error: any) {
        console.error("Error creating todo item:", error);
        notification.error({ message: "Failed to create todo item!" });
      }
    });
  };

  return (
    <Modal
      title={"Create Todo Item"}
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
