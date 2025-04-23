import React, { useEffect } from "react";
import {
  Form,
  Input,
  notification,
  Select,
  Button,
  Space,
  Typography,
} from "antd";
import { useTodoGroupStore } from "./todoGroupStore";
import { useNavigate } from "react-router-dom";
import { TodoGroupService } from "./todoGroupService";

const TodoGroupCreate: React.FC = () => {
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { createTodoGroup, createErrors } = useTodoGroupStore();

  const handleSaveTodoGroup = () => {
    form.validateFields().then(async (values) => {
      try {
        const data = await createTodoGroup(values);
        if (data) {
          notification.success({ message: "Todo list created successfully!" });
          form.resetFields();
          navigate("/todo-list");
        }
      } catch (error) {
        console.error("Error creating todo list:", error);
        notification.error({ message: "Failed to create todo list!" });
      }
    });
  };
  useEffect(() => {
    const fields = Object.entries(createErrors).map(([name, errors]) => ({
      name: name.toLowerCase(),
      errors,
    }));
    form.setFields(fields);
  }, [createErrors, form]);

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Todo List Create
        </Typography.Title>
      </Space>
      <Form form={form} layout="vertical" onFinish={handleSaveTodoGroup}>
        <Form.Item
          label="Todo Group Name"
          name="title"
          rules={[
            { required: true, message: "Please enter the todo list name!" },
          ]}
        >
          <Input placeholder="Enter todo list name" />
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
    </>
  );
};

export default TodoGroupCreate;
