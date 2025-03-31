/* eslint-disable @typescript-eslint/no-explicit-any */
import React from "react";
import { Form, Input, notification, Select, Button, Space } from "antd";
import { TodoGroupService } from "./todoGroupService";
import { useNavigate } from "react-router-dom";

const TodoGroupCreate: React.FC = () => {
  const [form] = Form.useForm();
  const navigate = useNavigate();

  const handleSaveTodoGroup = () => {
    form.validateFields().then(async (values) => {
      try {
        await TodoGroupService.createTodoGroup(values);
        notification.success({ message: "Operation successful!" });
        form.resetFields();
      } catch (error: any) {
        console.error("Error creating todo group:", error);
        notification.error({ message: "Failed to create todo group!" });
      }
    });
  };

  return (
    <Form form={form} layout="vertical" onFinish={handleSaveTodoGroup}>
      <Form.Item
        label="Todo List Title"
        name="title"
        rules={[
          { required: true, message: "Please enter the todo list title!" },
        ]}
      >
        <Input placeholder="Enter todo list title" />
      </Form.Item>
      <Form.Item
        label="Select Colour"
        name="colour"
        rules={[{ required: true, message: "Please select a colour!" }]}
      >
        <Select optionLabelProp="label">
          {TodoGroupService.getColours().map((colour) => (
            <Select.Option
              key={colour.value}
              value={colour.value}
              label={
                <span style={{ display: "flex", alignItems: "center" }}>
                  <span
                    style={{
                      display: "inline-block",
                      width: 20,
                      height: 20,
                      backgroundColor: colour.value,
                      marginRight: 10,
                      borderRadius: "50%",
                    }}
                  />
                  {colour.label}
                </span>
              }
            >
              <span style={{ display: "flex", alignItems: "center" }}>
                <span
                  style={{
                    display: "inline-block",
                    width: 20,
                    height: 20,
                    backgroundColor: colour.value,
                    marginRight: 10,
                    borderRadius: "50%",
                  }}
                />
                {colour.label}
              </span>
            </Select.Option>
          ))}
        </Select>
      </Form.Item>
      <Form.Item>
        <Space>
          <Button type="primary" htmlType="submit">
            Submit
          </Button>
          <Button type="default" onClick={() => navigate("/todo-list")}>
            Cancel
          </Button>
        </Space>
      </Form.Item>
    </Form>
  );
};

export default TodoGroupCreate;
