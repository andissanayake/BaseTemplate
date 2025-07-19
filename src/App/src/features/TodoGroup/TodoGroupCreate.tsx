import React from "react";
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
import { apiClient } from "../../common/apiClient";
import { handleFormValidationErrors } from "../../common/formErrorHandler";

const TodoGroupCreate: React.FC = () => {
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { setLoading } = useTodoGroupStore();

  const getColours = () => {
    const predefinedColours = [
      { label: "White", value: "#FFFFFF" },
      { label: "Red", value: "#FF5733" },
      { label: "Orange", value: "#FFC300" },
      { label: "Yellow", value: "#FFFF66" },
      { label: "Green", value: "#CCFF99" },
      { label: "Blue", value: "#6666FF" },
      { label: "Purple", value: "#9966CC" },
      { label: "Grey", value: "#999999" },
    ];
    return predefinedColours;
  };

  const handleSaveTodoGroup = () => {
    form.validateFields().then(async (values) => {
      setLoading(true);
      apiClient.post<number>("/api/todoLists", values, {
        onSuccess: () => {
          notification.success({ message: "Todo list created successfully!" });
          form.resetFields();
          navigate("/todo-list");
        },
        onValidationError: (createErrors) => {
          handleFormValidationErrors({
            form,
            errors: createErrors,
          });
        },
        onServerError: () => {
          notification.error({ message: "Failed to create todo list!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      });
    });
  };

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Todo List Create
        </Typography.Title>
      </Space>
      <Form form={form} layout="vertical" onFinish={handleSaveTodoGroup}>
        <Form.Item
          label="Todo List Name"
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
            {getColours().map((colour) => (
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
