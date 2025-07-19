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
import { useNavigate, useParams } from "react-router-dom";
import { apiClient } from "../../common/apiClient";
import { useAsyncEffect } from "../../common/useAsyncEffect";
import { handleFormValidationErrors } from "../../common/formErrorHandler";
import { TodoGroup } from "./TodoGroupModel";

const TodoGroupEdit: React.FC = () => {
  const { setLoading } = useTodoGroupStore();

  const [form] = Form.useForm();
  const navigate = useNavigate();
  const { listId } = useParams();
  if (!listId) throw new Error("listId is required");

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
      values.id = +listId;
      setLoading(true);
      apiClient.put<boolean>(`/api/todoLists/${values.id}`, values, {
        onSuccess: () => {
          notification.success({
            message: "Todo list updated successfully!",
          });
          navigate("/todo-list");
        },
        onValidationError: (updateErrors) => {
          handleFormValidationErrors({
            form,
            errors: updateErrors,
          });
        },
        onServerError: () => {
          notification.error({ message: "Failed to update todo list!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      });
    });
  };

  useAsyncEffect(async () => {
    form.resetFields();
    setLoading(true);
    apiClient.get<TodoGroup>(`/api/todoLists/${listId}`, {
      onSuccess: (data) => {
        form.setFieldsValue(data);
      },
      onServerError: () => {
        notification.error({ message: "Failed to fetch todo list item!" });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  }, [listId, form]);

  return (
    <>
      <Space className="mb-4">
        <Typography.Title level={3} style={{ margin: 0 }}>
          Todo List Edit
        </Typography.Title>
      </Space>
      <Form form={form} layout="vertical" onFinish={handleSaveTodoGroup}>
        <Form.Item
          label="Todo list Name"
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

export default TodoGroupEdit;
