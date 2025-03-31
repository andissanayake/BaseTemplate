/* eslint-disable @typescript-eslint/no-explicit-any */
import React from "react";
import { Modal, Form, Input, notification, DatePicker, Select } from "antd";
import { PriorityLevel, TodoItemService } from "./todoItemService";
import { useParams } from "react-router-dom";
import dayjs from "dayjs";

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
        await TodoItemService.createTodoItem({
          ...values,
          reminder: values.reminder ? dayjs(values.reminder).format() : null,
          listId: +listId,
        });
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
          label="Title"
          name="title"
          rules={[{ required: true, message: "Please enter the title!" }]}
        >
          <Input placeholder="Enter todo item title" />
        </Form.Item>

        <Form.Item label="Note" name="note">
          <Input.TextArea rows={3} placeholder="Optional note..." />
        </Form.Item>

        <Form.Item label="Reminder" name="reminder">
          <DatePicker
            style={{ width: "100%" }}
            showTime
            format="YYYY-MM-DD HH:mm"
          />
        </Form.Item>

        <Form.Item
          label="Priority Level"
          name="priority"
          initialValue={PriorityLevel.None}
        >
          <Select options={TodoItemService.getPriorityLevels()} />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default TodoItemCreate;
