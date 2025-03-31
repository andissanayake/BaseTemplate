/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect } from "react";
import { useTodoItemStore } from "./todoItemStore";
import { Button, Form, Input, notification, Space } from "antd";
import { TodoItemService } from "./todoItemService";

export const TodoItemEdit = () => {
  const { editTodoItem, setTodoItemEdit, fetchTodoItems } = useTodoItemStore();
  const [form] = Form.useForm();

  useEffect(() => {
    if (editTodoItem?.id) {
      form.setFieldsValue(editTodoItem);
    }
  }, [editTodoItem, form]);

  const handleSaveTodoItem = () => {
    form.validateFields().then(async (values) => {
      values.id = editTodoItem?.id;

      try {
        await TodoItemService.updateTodoItem(values);
        notification.success({ message: "Operation successful!" });
        setTodoItemEdit(null);
        await fetchTodoItems();
      } catch (error: any) {
        console.error("Error updating todo item:", error);
        notification.error({ message: "Failed to update todo item!" });
      }
    });
  };

  return (
    <div>
      <Form form={form} layout="vertical" onFinish={handleSaveTodoItem}>
        <Form.Item
          label="Todo Item Name"
          name="title"
          rules={[
            { required: true, message: "Please enter the todo item name!" },
          ]}
        >
          <Input placeholder="Enter todo item name" />
        </Form.Item>

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit">
              Submit
            </Button>
            <Button type="default" onClick={() => setTodoItemEdit(null)}>
              Cancel
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </div>
  );
};
